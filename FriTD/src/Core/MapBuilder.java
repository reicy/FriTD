package Core;

import java.util.LinkedList;

import Entities.EmptyTowerSquare;
import Entities.MapSquare;
import Entities.PathSquare;
import Enums.MapSquareType;
import UI.IDisplayableObject;

public class MapBuilder {
	
	private final int MAP_SQUARE_SIZE = 30;
	
	private MapSquare[][] map;

	public void buildMap(String string) {
		map = new MapSquare[10][10];
		for (int i = 0; i < map.length; i++) {
			for (int j = 0; j < map[0].length; j++) {
				map[i][j] = new MapSquare(MapSquareType.grassSquare, MAP_SQUARE_SIZE,i,j);
			}
		}
		
		for (int j = 0; j < map[0].length; j++) {
			map[5][j] = new MapSquare(MapSquareType.pathSquare, MAP_SQUARE_SIZE,4,j);
		}
		
		for (int j = 0; j < map[0].length; j++) {
			map[3][j] = new MapSquare(MapSquareType.towerPlaceSquare, MAP_SQUARE_SIZE,5,j);
		}
		
		for (int j = 0; j < map[0].length; j++) {
			map[4][j] = new MapSquare(MapSquareType.towerPlaceSquare, MAP_SQUARE_SIZE,3,j);
		}
		
		
	}

	public PathSquare getFirstPathSquare() {
		// TODO Auto-generated method stub
		return null;
	}

	public LinkedList<EmptyTowerSquare> getEmptyTowerSquares() {
		// TODO Auto-generated method stub
		return null;
	}

	public MapSquare[][] getMap() {
		return map;
	}

}
