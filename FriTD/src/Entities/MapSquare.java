package Entities;

import java.awt.Graphics2D;

import Enums.MapSquareType;
import UI.IDisplayableObject;

public class MapSquare implements IDisplayableObject{
	
	private MapSquareType type;
	private int size,row,col;
	
	

	public MapSquare(MapSquareType type, int size, int row, int col) {
		super();
		this.type = type;
		this.size = size;
		this.row = row;
		this.col = col;
	}




	@Override
	public void display(Graphics2D g2d) {
		// TODO Auto-generated method stub
		
	}

}
