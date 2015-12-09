package Enums;

public enum TowerType {

	frost(10), venom(15), standard(20);
	
	private int cost;

	private TowerType(int cost) {
		this.cost = cost;
	}

	public int getCost() {
		return cost;
	}
	
	
	
}
