using UnityEngine;
using System.Collections;
using System;

namespace Umbra.Data {
	public enum StarMapLevel { StarMap01, StarMap02, StarMap09, StarMapQ1, StarMapQ2, StarMapQ4, StarMapQ5, StarMapQ6, StarMapQ7 }
	public enum GameScene { MainMenu, NewCharacter, StarMap, ExplorationMap, ShipMenu, ReputationMenu, TradeMenu, ShipUpgradeMenu, OutpostMenu, CharacterMenu, CreateUnitMenu, RosterMenu, BattleMap, LoadGame, SettingsMenu}
	public enum MapButtons { ExplorationMap, BattleMap, Starmap, WormholeMap };
	public enum BattleButtons { cmdAutoplay, cmdFlee, cmdReturn, ctxInspect, ctxAbility1, ctxAbility2, ctxAbility3, ctxAbility4, ctxAbility5, ctxAbility6, ctxItem1, ctxItem2 };
	public enum ExplorationButtons { cmdObjectives, ctxInspect, ctxRecruitAttack, ctxTalkUse };

    public enum SpawnType
    {
        Character,
		Unit,
		Friendly,
        Enemy,
        Item
    }
}