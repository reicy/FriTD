package Core;

import java.util.LinkedList;

import Entities.EmptyTowerSquare;
import Entities.MapSquare;
import Entities.PathSquare;
import Enums.MapSquareType;
import UI.IDisplayableObject;

public class MapBuilder {
	
	private final int MAP_SQUARE_SIZE = 30;
	
	private IDisplayableObject[][] map;
	private PathSquare firstPathSquare;
	private LinkedList<EmptyTowerSquare> towerSquares;
	

	//TODO generate from file
	public void buildMap(String mapFile) {
		
		towerSquares = new LinkedList<>();
		map = new IDisplayableObject[10][10];
		for (int i = 0; i < map.length; i++) {
			for (int j = 0; j < map[0].length; j++) {
				map[i][j] = new MapSquare(MapSquareType.grassSquare, MAP_SQUARE_SIZE,i,j);
			}
		}
		
		for (int j = 0; j < map[0].length; j++) {
			map[5][j] = new MapSquare(MapSquareType.pathSquare, MAP_SQUARE_SIZE,5,j);
		}
		
		PathSquare pathSquare;
		firstPathSquare = new PathSquare(0, 5*MAP_SQUARE_SIZE);
		pathSquare = firstPathSquare;
		
		for (int i = 1; i < map[5].length; i++) {
			pathSquare.addNext(new PathSquare(i*MAP_SQUARE_SIZE+MAP_SQUARE_SIZE/2, 5*MAP_SQUARE_SIZE+MAP_SQUARE_SIZE/2));
			pathSquare = pathSquare.getNext();
		}
		
		
		for (int j = 0; j < map[0].length; j++) {
			map[4][j] = new MapSquare(MapSquareType.towerPlaceSquare, MAP_SQUARE_SIZE,4,j);
			towerSquares.addLast(new EmptyTowerSquare(j*MAP_SQUARE_SIZE, 4*MAP_SQUARE_SIZE, 4*100+j));
		}
		
		
		
		for (int j = 0; j < map[0].length; j++) {
			map[6][j] = new MapSquare(MapSquareType.towerPlaceSquare, MAP_SQUARE_SIZE,6,j);
			towerSquares.addLast(new EmptyTowerSquare(j*MAP_SQUARE_SIZE, 6*MAP_SQUARE_SIZE, 6*100+j));
		}
		
		
		
		
	}

	public PathSquare getFirstPathSquare() {
		return firstPathSquare;
	}

	public LinkedList<EmptyTowerSquare> getEmptyTowerSquares() {
		return towerSquares;
	}

	public IDisplayableObject[][] getMap() {
		return map;
	}

}
