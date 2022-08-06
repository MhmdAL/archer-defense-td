using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Linq;

[ExecuteInEditMode]
public class InfoPanel : MonoBehaviour {

	public int enemyID;

	private Monster associatedEnemy;

	public TextMeshProUGUI enemyTitleText, enemyDescText;

	public Image hpStarRating, armorStarRating, enemyImage;

	private InfoPanelManager ipm;
	private MonsterManager m;

	void Start(){
		ipm = ValueStore.Instance.infoPanelManagerInstance;
		m = ValueStore.Instance.monsterManagerInstance;

		// associatedEnemy = m.enemyPrefabs.FirstOrDefault (x => x.ID == enemyID);

		// enemyImage.sprite = associatedEnemy.icon;

		// enemyTitleText.text = associatedEnemy.EnemyData.Name;
		//enemyDescText.text = associatedEnemy.description;

		// float hpFill = (float)associatedEnemy.MaxHP.Value / (float)m.maximumHP;
		// float armorFill = (float)associatedEnemy.Armor.Value / (float)m.maximumArmor;

		// hpStarRating.fillAmount = Mathf.Clamp (hpFill, 0, 1);
		// armorStarRating.fillAmount = Mathf.Clamp (armorFill, 0, 1);

		if(Application.isPlaying)
			Destroy (transform.parent.gameObject, ipm.delayTillDeath);
	}
}
