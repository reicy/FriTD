package Core;

import Entities.Enemy;
import Entities.PathSquare;

public class Spawner {
	
	
	private int counter;
	//TODO replace with automatic eval
	private int creatureDelay;
	private int nextCreatureId = 0;
	private int toSpawn;
	private CreatureBuilder cbuilder;

	//TODO from file
	public Enemy spawnCreature(PathSquare firstPathSquare) {
		
		return cbuilder.getEnemy(firstPathSquare, nextCreatureId++);
		
		
	}

	public boolean isTimeToSpawn() {
		if(toSpawn<=0)return false;
		counter--;
		if(counter <= 0){
			counter = creatureDelay;
			toSpawn--;
			return true;
		}
		return false;
	}
	
	public void startSpawning(){
		toSpawn = 20;
		cbuilder.changeToNextTemplate();
	}

	

}
