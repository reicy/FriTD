package Entities;

import java.awt.Color;
import java.awt.Graphics2D;
import java.util.LinkedList;

import Core.ProjectileBuilder;
import Helpers.MathHelper;
import UI.IDisplayableObject;

public class Tower  implements IDisplayableObject{
	
	private int x,y;
	private int id;
	private Color color;
	private ProjectileBuilder battery;
	private int range;
	private int reloadTime;
	private int counter;
	
	
	

	public Tower(int x, int y, int id, Color color, ProjectileBuilder battery, int range, int reloadTime) {
		super();
		this.x = x;
		this.y = y;
		this.id = id;
		this.color = color;
		this.battery = battery;
		this.range = range;
		this.reloadTime = reloadTime;
	}

	@Override
	public void display(Graphics2D g2d) {

		Color c = g2d.getColor();
		g2d.setColor(color);
		g2d.fillOval(x-10, y-10, 20, 20);
		g2d.setColor(c);
		
	}

	public Projectile tryToFireAtFirstEnemyInSight(LinkedList<Enemy> enemies) {
		if(counter > 0){
			counter--;
		}
		if(counter>0)return null;
		
		Projectile projectile = null;
		for (Enemy enemy : enemies) {
			if(MathHelper.getDistanceBetweenPoints(enemy.getX(), enemy.getY(), x, y)<=range){
				projectile = battery.buildLockAndLoad(enemy);
				counter = reloadTime;
				break;
			};
		}
		
		return projectile;
		
	}

}
