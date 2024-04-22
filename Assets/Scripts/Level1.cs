using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shapes2D;
using UnityEngine;
using UnityTimer;

public class Level1 : MonoBehaviour
{
    public GameUIController uIController;
    public ValueStore gameController;
    public AbilityUsedEvent abilityUsedEvent;

    public List<TutorialStage> stages;
    private GameObject _pathMask;

    private Level1Stage currentStage;

    [SerializeField]
    private float initialStageDelay = 1.5f;
    [SerializeField]
    private float cavalryRaidWaitTime = 3f;

    public TutorialStage CurrentStage => stages.First(x => x.StageId == currentStage);

    private void Awake()
    {
        _pathMask = GameObject.Find("PathMaskParent");

        uIController = FindObjectOfType<GameUIController>();
        
        InitStages();

        // uIController.StartWaveEnabled = false;
    }

    private void Start()
    {
        uIController.horseRaidAbility.SetActive(false);
        uIController.HideWaveMenu();
        uIController.HideTimeControls();

        gameController = FindObjectOfType<ValueStore>();

        gameController.userClickHandlerInstance.ObjectClicked += OnObjectClicked;
        gameController.WaveSpawner.WaveStarted += OnWaveStarted;

        abilityUsedEvent.Subscribe(OnAbilityUsed);

        uIController.horseRaidAbility.GetComponentInChildren<AbilityHorseRaidV2>().RaidStarted += OnRaidStarted;

        this.AttachTimer(initialStageDelay, (t) =>
        {
            currentStage = (Level1Stage)1;
            stages[0].Begin();
        });
    }

    private void InitStages()
    {
        stages = new List<TutorialStage>
        {
            new TutorialStage
            {
                StageId = Level1Stage.Intro,
                Begin = () => {
                    uIController.EnableBlur();

                    uIController.tutorialManager.SetActiveStage((int)Level1Stage.Intro);
                },
                End = () => {
                    uIController.DisableBlur();
                }
            },
            new TutorialStage
            {
                StageId = Level1Stage.WaveLivesInfo,
                Begin = () => {
                    uIController.EnableBlur();

                    uIController.tutorialManager.SetActiveStage((int)Level1Stage.WaveLivesInfo);
                    uIController.GO_statsPanel.transform.parent = uIController.GO_statsPanel.transform.parent.parent;
                    uIController.GO_statsPanel.transform.SetAsLastSibling();

                    // uIController.GO_timeControlsPanel.transform.parent = uIController.GO_timeControlsPanel.transform.parent.parent;
                    // uIController.GO_timeControlsPanel.transform.SetAsLastSibling();
                    // uIController.GO_timeControlsPanel.SetActive(false);
                },
                End = () => {
                    uIController.DisableBlur();

                    uIController.GO_statsPanel.transform.parent = uIController.GO_HUD.transform;
                    // uIController.GO_timeControlsPanel.transform.parent = uIController.GO_HUD.transform;

                    // uIController.GO_timeControlsPanel.SetActive(true);
                }
            },
            new TutorialStage
            {
                StageId = Level1Stage.SelectArcher,
                Begin = () => {
                    uIController.tutorialManager.DisableBackdrop();

                    uIController.tutorialManager.SetActiveStage((int)Level1Stage.SelectArcher);
                },
                End = () => {
                    uIController.tutorialManager.EnableBackdrop();
                }
            },
            new TutorialStage
            {
                StageId = Level1Stage.MoveArcher,
                Begin = () => {
                    uIController.tutorialManager.SetActiveStage((int)Level1Stage.MoveArcher);
                }
            },
            new TutorialStage
            {
                StageId = Level1Stage.FiringModes,
                Begin = () => {
                    uIController.tutorialManager.SetActiveStage((int)Level1Stage.FiringModes);
                }
            },
            new TutorialStage
            {
                StageId = Level1Stage.StartWave,
                Begin = () => {
                    uIController.tutorialManager.SetActiveStage((int)Level1Stage.StartWave);

                    uIController.ShowWaveMenu();

                    uIController.GO_spawnWavePanelParent.transform.parent = uIController.GO_statsPanel.transform.parent.parent;
                    uIController.GO_spawnWavePanelParent.transform.SetAsLastSibling();
                },
                End = () => {
                    uIController.GO_spawnWavePanelParent.transform.parent = uIController.GO_HUD.transform;
                }
            },
            new TutorialStage
            {
                StageId = Level1Stage.UseRaid,
                Begin = () => {
                    uIController.horseRaidAbility.SetActive(true);

                    uIController.tutorialManager.SetActiveStage((int)Level1Stage.UseRaid);

                    uIController.horseRaidAbility.transform.parent = uIController.GO_statsPanel.transform.parent.parent;
                    uIController.horseRaidAbility.transform.SetAsLastSibling();
                },
                End = () => {
                    uIController.horseRaidAbility.transform.parent = uIController.GO_HUD.transform;
                }
            },
            new TutorialStage
            {
                StageId = Level1Stage.ActivateRaid,
                Begin = () => {
                    _pathMask.transform.GetChild(0).gameObject.SetActive(true);
                    uIController.tutorialManager.SetActiveStage((int)Level1Stage.ActivateRaid, 0);
                },
                End = () => {
                    _pathMask.transform.GetChild(0).gameObject.SetActive(false);

                    uIController.ShowTimeControls();
                }
            }
        };
    }

    private void OnObjectClicked(object obj)
    {
        if (obj is Tower && currentStage == Level1Stage.SelectArcher && !CurrentStage.StageFinished)
        {
            EndStage();

            NextStage();
        }
    }

    private void OnWaveStarted(int wave)
    {
        if(currentStage == Level1Stage.StartWave && wave == 1)
        {
            EndStage();
        }

        if (currentStage == Level1Stage.StartWave && wave == 2)
        {
            this.AttachTimer(cavalryRaidWaitTime, (f) => NextStage());
        }
    }

    private void OnAbilityUsed(AbilityType abilityType)
    {
        print("ability activated");

        if (currentStage == Level1Stage.UseRaid && abilityType == AbilityType.Cavalry_Raid)
        {
            print("ending stg 5");
            EndStage();

            NextStage();
        }
    }

    private void OnRaidStarted()
    {
        if (currentStage == Level1Stage.ActivateRaid)
        {
            EndStage();
        }
    }

    private void EndStage()
    {
        CurrentStage.StageFinished = true;
        uIController.tutorialManager.Close();

        CurrentStage.End?.Invoke();
    }

    private void NextStage()
    {
        if (stages.Count > (int)currentStage)
        {
            currentStage++;
            CurrentStage.Begin();
        }
    }

    float totalWalkTime = 0;

    private void Update()
    {
        if (currentStage is Level1Stage.Intro or Level1Stage.WaveLivesInfo or Level1Stage.FiringModes && Input.GetMouseButtonDown(0) && !CurrentStage.StageFinished)
        {
            EndStage();

            NextStage();
        }
        else if (currentStage == Level1Stage.MoveArcher && !CurrentStage.StageFinished && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
        {
            totalWalkTime += Time.deltaTime;

            if (totalWalkTime > 2f)
            {
                EndStage();

                this.AttachTimer(1.5f, (t2) =>
                {
                    NextStage();
                });
            }
        }

    }
}

public class TutorialStage
{
    public Level1Stage StageId;
    public bool StageFinished;
    public string Message;
    public Action Begin;
    public Action End;
}

public enum Level1Stage
{
    Intro = 1,
    WaveLivesInfo,
    SelectArcher,
    MoveArcher,
    FiringModes,
    StartWave,
    UseRaid,
    ActivateRaid,
}