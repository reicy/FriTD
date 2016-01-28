package Entities;

public class PathSquare {

	private int x,y;
	private PathSquare next;
	private PathSquare previous;
	
	
	
	public PathSquare(int x, int y) {
		super();
		this.x = x;
		this.y = y;
	}

	public void addNext(PathSquare n){
		next = n;
		n.previous = this;
	}
	
	public PathSquare getNext(){
		return next;
	}

	public int getX() {
		return x;
	}

	public void setX(int x) {
		this.x = x;
	}

	public int getY() {
		return y;
	}

	public void setY(int y) {
		this.y = y;
	}
	
	
	
	
}
