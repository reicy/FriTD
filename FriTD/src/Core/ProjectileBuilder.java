package Core;

import Entities.Effect;
import Entities.Enemy;
import Entities.Projectile;
import Enums.TowerType;

public class ProjectileBuilder {
	
	private TowerType type;
	private int x,y;

	public ProjectileBuilder(TowerType type, int x, int y) {
		this.type = type;
		this.x = x;
		this.y = y;
	}

	public Projectile buildLockAndLoad(Enemy enemy) {
		Projectile projectile;
		projectile = new Projectile(10, new Effect(type.getDuration(), type.getDmg(), type.getSlow()), x, y, enemy);
		return projectile;
	}

}
