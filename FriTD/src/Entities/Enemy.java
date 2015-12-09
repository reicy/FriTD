package Entities;

import java.awt.Color;
import java.awt.Graphics2D;
import java.util.LinkedList;

import Enums.EnemyState;
import Helpers.MathHelper;
import UI.IDisplayableObject;

public class Enemy implements IDisplayableObject {
	
	private int id;
	private int x,y;
	private int hp;
	private int maxHp;
	private int speed;
	private int goldValue;
	private int HPCost;
	private EnemyState state;
	private PathSquare squareWayPoint;
	private LinkedList<Effect> effects;
	private int maxSpeed;
	
	
	

	public int getX() {
		return x;
	}


	
	public int getY() {
		return y;
	}



	public Enemy(int id, int x, int y, int hp, int speed, int goldValue, int hPCost, PathSquare squareWayPoint) {
		super();
		this.id = id;
		this.x = x;
		this.y = y;
		this.hp = hp;
		maxHp = hp;
		this.speed = speed;
		this.goldValue = goldValue;
		HPCost = hPCost;
		this.squareWayPoint = squareWayPoint;
		this.state = EnemyState.alive;
		this.effects = new LinkedList<>();
		this.maxSpeed = speed;
	}
	

	@Override
	public void display(Graphics2D g2d) {
		Color c = g2d.getColor();
		g2d.setColor(Color.black);
		g2d.fillOval(x-7, y-7, 15, 15);
		g2d.setColor(Color.red);
		g2d.fillRect(x-6, y-1, (int) (14*(hp*1.0/maxHp*1.0)), 2);
		g2d.setColor(c);
		
	}

	public void move() {
		LinkedList<Effect> expiredEffects = new LinkedList<>();;
		speed = maxSpeed;
		//add here projectile effect application
		for (Effect effect : effects) {
			speed = (int) ((int) speed*effect.getSlow());
			hp-=effect.getDmg();
			effect.reduceTTL();
			if(effect.getRemainingTurns()<=0)expiredEffects.addLast(effect);
		}
		if(hp<=0){
			this.state = EnemyState.dead;
			
		}
		for (Effect effect : expiredEffects) {
			effects.remove(effect);
		}
		
		if(state == EnemyState.dead)return;
		
		
		if(squareWayPoint == null){
			this.state = EnemyState.victorious;
			return;
		}
		
		
		int nextX, nextY;
		
		if(squareWayPoint.getX()!=x){
			if(squareWayPoint.getX() < x){
				nextX = x - speed;
			}else{
				nextX = x + speed;
			}
		}else{
			nextX = x;
		}
		if(squareWayPoint.getY()!=y){
			if(squareWayPoint.getY() < y){
				nextY = y - speed;
			}else{
				nextY = y + speed;
			}
		}else{
			nextY = y;
		}
		
		//MCH
		if(MathHelper.getDistanceBetweenPoints(nextX, nextY, squareWayPoint.getX(), squareWayPoint.getY())<2*speed){
			squareWayPoint = squareWayPoint.getNext();
			
		};
		
		x = nextX;
		y = nextY;
		
	}

	public boolean isDead() {
		
		return state == EnemyState.dead;
	}

	public int getGoldValue() {
		return goldValue;
	}

	public int getHPCost() {
		
		return HPCost;
	}

	public boolean isVictorious() {
		return state == EnemyState.victorious;
	}

	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result + id;
		return result;
	}

	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (getClass() != obj.getClass())
			return false;
		Enemy other = (Enemy) obj;
		if (id != other.id)
			return false;
		return true;
	}



	public void applyEffect(Effect effect) {
		this.effects.addLast(effect);
	}

	
	
}
