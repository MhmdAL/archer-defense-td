using System;
using System.Collections;
using System.Collections.Generic;
using Shapes2D;
using UnityEngine;
using UnityTimer;

public enum LevelState
{
    Wave1,
    Wave2,
    Wave3
}

public class Level1 : MonoBehaviour
{
    public GameUIController uIController;
    public ValueStore gameController;

    public List<TutorialStage> stages;
    private GameObject _pathMask;

    private int currentStage = 0;

    public TutorialStage CurrentStage => stages[currentStage];

    private void Awake()
    {
        _pathMask = GameObject.Find("PathMaskParent");

        uIController = FindObjectOfType<GameUIController>();

        stages = new List<TutorialStage>
        {
            new TutorialStage
            {
                Begin = () => {
                    uIController.tutorialManager.SetActiveStage(1);
                }
            },
            new TutorialStage
            {
                Begin = () => {
                    uIController.tutorialManager.SetActiveStage(2);
                    uIController.GO_statsPanel.transform.parent = uIController.GO_statsPanel.transform.parent.parent;
                    uIController.GO_statsPanel.transform.SetAsLastSibling();

                    uIController.GO_timeControlsPanel.transform.parent = uIController.GO_timeControlsPanel.transform.parent.parent;
                    uIController.GO_timeControlsPanel.transform.SetAsLastSibling();
                },
                End = () => {
                    uIController.GO_statsPanel.transform.parent = uIController.GO_HUD.transform;
                    uIController.GO_timeControlsPanel.transform.parent = uIController.GO_HUD.transform;
                }
            },
            new TutorialStage
            {
                Begin = () => {
                    uIController.tutorialManager.SetActiveStage(3);
                }
            },
            new TutorialStage
            {
                Begin = () => {
                    uIController.tutorialManager.SetActiveStage(4);
                }
            },
            new TutorialStage
            {
                Begin = () => {
                    uIController.tutorialManager.SetActiveStage(5);

                    uIController.GO_spawnWavePanelParent.transform.parent = uIController.GO_statsPanel.transform.parent.parent;
                    uIController.GO_spawnWavePanelParent.transform.SetAsLastSibling();
                },
                End = () => {
                    uIController.GO_spawnWavePanelParent.transform.parent = uIController.GO_HUD.transform;
                }
            },
            new TutorialStage
            {
                Begin = () => {
                    uIController.tutorialManager.SetActiveStage(6);

                    uIController.horseRaidAbility.transform.parent = uIController.GO_statsPanel.transform.parent.parent;
                    uIController.horseRaidAbility.transform.SetAsLastSibling();
                },
                End = () => {
                    uIController.horseRaidAbility.transform.parent = uIController.GO_HUD.transform;
                }
            },
            new TutorialStage
            {
                Begin = () => {
                    _pathMask.transform.GetChild(0).gameObject.SetActive(true);
                    uIController.tutorialManager.SetActiveStage(7, 0);
                },
                End = () => {
                    _pathMask.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        };

        // uIController.StartWaveEnabled = false;
    }

    private void Start()
    {

        gameController = FindObjectOfType<ValueStore>();

        gameController.userClickHandlerInstance.ObjectClicked += OnObjectClicked;
        gameController.WaveSpawner.WaveStarted += OnWaveStarted;

        Ability.AbilityActivated += OnAbilityActivated;

        uIController.horseRaidAbility.GetComponentInChildren<AbilityHorseRaidV2>().RaidStarted += OnRaidStarted;

        this.AttachTimer(1f, (t) =>
        {
            stages[0].Begin();
        });
    }

    private void OnObjectClicked(object obj)
    {
        if (obj is Tower && currentStage == 2 && !CurrentStage.StageFinished)
        {
            EndStage();

            NextStage();
        }
    }

    private void OnWaveStarted(int wave)
    {
        if (currentStage == 4 && !CurrentStage.StageFinished)
        {
            EndStage();
        }

        if (currentStage == 4 && CurrentStage.StageFinished && wave == 2)
        {
            NextStage();
        }
    }

    private void OnAbilityActivated(AbilityType abilityType)
    {
        if (currentStage == 5 && abilityType == AbilityType.Cavalry_Raid)
        {
            EndStage();

            NextStage();
        }
    }

    private void OnRaidStarted()
    {
        if (currentStage == 6)
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
        if (stages.Count > currentStage + 1)
        {
            currentStage++;
            stages[currentStage].Begin();
        }
    }

    float totalWalkTime = 0;

    private void Update()
    {
        if (currentStage is 0 or 1 && Input.GetMouseButtonDown(0))
        {
            EndStage();

            NextStage();
        }
        else if (currentStage == 3 && !CurrentStage.StageFinished && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
        {
            totalWalkTime += Time.deltaTime;

            if (totalWalkTime > 2f)
            {
                EndStage();

                uIController.ShowWaveMenu(false);

                this.AttachTimer(1.5f, (t2) =>
                {
                    uIController.BTN_spawnWave.interactable = true;

                    NextStage();
                });
            }
        }

    }
}

public class TutorialStage
{
    public bool StageFinished;
    public string Message;
    public Action Begin;
    public Action End;
}
