using UnityEngine;

public class MineBuilding : Building {
    public GameObject MoneyRobotPrefab; //挖金机器人
    public Transform SpawnPos;
    
    private MineRobotAI mMineRobot;

	protected override void Start () {
        base.Start();

        GameObject moneyRobotGo = Instantiate(MoneyRobotPrefab, transform) as GameObject;
        moneyRobotGo.transform.position = SpawnPos.position;
        moneyRobotGo.GetComponent<NavMeshAgent>().Warp(SpawnPos.position);
        mMineRobot = moneyRobotGo.GetComponent<MineRobotAI>();
        mMineRobot.Init(this);
        
        mOwner.Init(gameObject, BuildingType.Mine);
	}
	
	public void CollectMoney(int num)
    {
        if(BelongSide() == Side.Team1)
        {
            Player.Instance.Money += num;
        }
        else if(BelongSide() == Side.Team2)
        {
            EnemyTeamAI.Instance.Money += num;
        }
    }

    void OnDestroy()
    {
        Destroy(mMineRobot.gameObject);
    }
}
