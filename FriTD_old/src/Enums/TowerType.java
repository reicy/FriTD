package Enums;

import java.awt.Color;

public enum TowerType {

	frost(10,10,1,0.8,4,Color.blue,50), venom(15,5,10,1,4,Color.green,50), standard(20,1,30,1,4, Color.DARK_GRAY,50);
	
	private int cost;
	private int duration;
	private int dmg;
	private double slow;
	private int reloadTime;
	private Color color;
	private int range;
	
	

	private TowerType(int cost, int duration, int dmg, double slow, int reloadTime, Color color, int range) {
		this.cost = cost;
		this.duration = duration;
		this.dmg = dmg;
		this.slow = slow;
		this.reloadTime = reloadTime;
		this.color = color;
		this.range = range;
	}




	public int getRange() {
		return range;
	}




	public Color getColor() {
		return color;
	}




	public int getDuration() {
		return duration;
	}




	public int getDmg() {
		return dmg;
	}




	public double getSlow() {
		return slow;
	}




	public int getReloadTime() {
		return reloadTime;
	}




	public int getCost() {
		return cost;
	}
	
	
	
}
