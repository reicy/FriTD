package Entities;

import java.awt.Color;
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
	
	public MapSquareType getType() {
		return type;
	}

	public void setType(MapSquareType type) {
		this.type = type;
	}
	public int getSize() {
		return size;
	}
	
	public void setSize(int size) {
		this.size = size;
	}

	public int getRow() {
		return row;
	}

	public void setRow(int row) {
		this.row = row;
	}

	public int getCol() {
		return col;
	}

	public void setCol(int col) {
		this.col = col;
	}

	@Override
	public void display(Graphics2D g2d) {
		
		
		if(type==MapSquareType.grassSquare) g2d.setColor(Color.GREEN);
		else if(type==MapSquareType.pathSquare) g2d.setColor(Color.YELLOW); 
		else if(type==MapSquareType.towerPlaceSquare) g2d.setColor(Color.GRAY);
		g2d.fillRect(size*col, size*row, size, size);
		g2d.setColor(Color.BLACK);
		g2d.drawRect(col*size, row*size, size, size);
		
	}

}
