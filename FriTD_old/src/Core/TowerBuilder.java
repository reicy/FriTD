package Core;

import Entities.Tower;
import Enums.TowerType;

public class TowerBuilder {

	public static Tower buildTower(int x, int y, int id, TowerType type) {
		Tower tower = null;
		
		
		tower = new Tower(x,y,id,type.getColor(),new ProjectileBuilder(type,x,y),type.getRange(), type.getReloadTime());
				
		return tower;
	}

	
}
