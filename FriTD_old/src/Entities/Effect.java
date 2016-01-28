package Entities;

public class Effect {

	private int remainingTurns;
	private int dmg;
	private double slow;
	
	public Effect(int remainingTurns, int dmg, double slow) {
		super();
		this.remainingTurns = remainingTurns;
		this.dmg = dmg;
		this.slow = slow;
	}

	public int getRemainingTurns() {
		return remainingTurns;
	}

	public int getDmg() {
		return dmg;
	}

	public double getSlow() {
		return slow;
	}

	public void reduceTTL() {
		remainingTurns--;
	}
	
	
	
	
}
