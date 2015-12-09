package Entities;

import java.awt.Graphics2D;

import Enums.MapSquareType;
import UI.IDisplayableObject;

public class MapSquare implements IDisplayableObject{
	
	private MapSquareType type;
	private int size;
	
	

	public MapSquare(MapSquareType type, int size) {
		super();
		this.type = type;
		this.size = size;
	}



	@Override
	public void display(Graphics2D g2d) {
		// TODO Auto-generated method stub
		
	}

}
