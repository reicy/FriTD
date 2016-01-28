package Core;

import Entities.Enemy;
import Entities.PathSquare;

public class CreatureBuilder {

	public Enemy getEnemy(PathSquare firstPathSquare, int nextCreatureId) {
		// TODO Auto-generated method stub
		return new Enemy(nextCreatureId, firstPathSquare.getX(), firstPathSquare.getY(), 100, 5, 10, 5, firstPathSquare.getNext());
		
	}

	public void changeToNextTemplate() {
		// TODO Auto-generated method stub
		
	}

}
