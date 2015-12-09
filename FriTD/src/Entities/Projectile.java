package Entities;

import java.awt.Color;
import java.awt.Graphics2D;

import Enums.ProjectileState;
import Helpers.MathHelper;
import UI.IDisplayableObject;

public class Projectile  implements IDisplayableObject{
	
	private int speed;
	private Effect effect;
	private int x,y;
	private ProjectileState state;
	private Enemy target;
	
	

	public Projectile(int speed,Effect effect, int x, int y, Enemy target) {
		super();
		this.speed = speed;
		this.effect = effect;
		this.x = x;
		this.y = y;
		state = ProjectileState.active;
		this.target = target;
	}

	@Override
	public void display(Graphics2D g2d) {
		Color c = g2d.getColor();
		g2d.setColor(Color.orange);
		g2d.fillOval(x-7, y-7, 15, 15);
		g2d.setColor(c);
		
	}

	public boolean isEmpty() {
		return state == ProjectileState.nonactive;
	}

	public void move() {
		if(ProjectileState.nonactive == state)return;
		int nextX, nextY;
		
		if(target.getX()!=x){
			if(target.getX() < x){
				nextX = x - speed;
			}else{
				nextX = x + speed;
			}
		}else{
			nextX = x;
		}
		if(target.getY()!=y){
			if(target.getY() < y){
				nextY = y - speed;
			}else{
				nextY = y + speed;
			}
		}else{
			nextY = y;
		}
		
		//MCH
		if(MathHelper.getDistanceBetweenPoints(x, y, target.getX(), target.getY())<speed){
			target.applyEffect(effect);
			this.effect = null;
			this.state = ProjectileState.nonactive;
			
		};
		
		x = nextX;
		y = nextY;
		
	}

}
