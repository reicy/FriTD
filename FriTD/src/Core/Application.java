package Core;

import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.LinkedList;

import javax.swing.Timer;

import Entities.EmptyTowerSquare;
import Entities.Enemy;
import Entities.PathSquare;
import Entities.Projectile;
import Entities.Tower;
import UI.IDisplayableObject;

public class Application implements ActionListener {
	
	private IDisplayableObject [][] map;
	private PathSquare firstPathSquare;
	private boolean isRoundRunning;
	private Timer timer;
	private LinkedList<Tower> towers;
	private LinkedList<Enemy> enemies;
	private LinkedList<Projectile> projectiles;
	private LinkedList<EmptyTowerSquare> emptyTowerSquares;
	private boolean gameRunning;
	private boolean victory;
	private int gold;
	private int hp;
	private MapBuilder mapBuilder;
	private LinkedList<Enemy> fallenOnes;
	private LinkedList<Enemy> victoriousOnes;
	private LinkedList<Projectile> emptyOnes;
	private Spawner spawner;
	

	
	public Application() {
		
		super();
		gold = 200;
		hp = 10000;
		projectiles = new LinkedList<>();
		enemies = new LinkedList<>();
		towers = new LinkedList<>();
		victoriousOnes = new LinkedList<>();
		timer = new Timer(100, this);
		isRoundRunning = false;
		mapBuilder = new MapBuilder();
		mapBuilder.buildMap("simpleMap.txt");
		firstPathSquare = mapBuilder.getFirstPathSquare();
		emptyTowerSquares = mapBuilder.getEmptyTowerSquares();
		map = mapBuilder.getMap();
		fallenOnes = new LinkedList<>();
		emptyOnes = new LinkedList<>();
		spawner = new Spawner();
		
	}
	
	
	
	
	


	
	public LinkedList<IDisplayableObject> getEnemies(){
		LinkedList<IDisplayableObject> items = new LinkedList<>();
		for (Enemy enemy : enemies) {
			items.addLast(enemy);
		}
		return items;
	};
	public LinkedList<IDisplayableObject> getProjectiles(){
		LinkedList<IDisplayableObject> items = new LinkedList<>();
		for (Projectile projectile : projectiles) {
			items.addLast(projectile);
		}
		return items;
	};
	
	public LinkedList<IDisplayableObject> getTowers(){
		LinkedList<IDisplayableObject> items = new LinkedList<>();
		for (Tower tower : towers) {
			items.addLast(tower);
		}
		return items;
	};
	
	
	public IDisplayableObject[][] getMap(){
		return map;
	};
	
	
	public void start(){
		if(!isRoundRunning){
			this.startRount();
		}
	};

	public int getMoney(){
		return 0;
	};
	public int getHP(){
		return 0;
	};
	public void buildTower(String position, String type){
		
	}




	@Override
	public void actionPerformed(ActionEvent e) {

		if(isRoundOver()){
			stopRound();
		}else{
			executeRoundIteration();
		}
		
	}




	private void executeRoundIteration() {
		
		//spawning pool
		if(spawner.isTimeToSpawn()){
			enemies.addLast(spawner.spawnCreature(firstPathSquare));
		}

		//give them fire
		for (Tower tower : towers) {
			tower.tryToFireAtFirstEnemyInSight(enemies);
		}
		
		//faster bastards
		for (Projectile projectile : projectiles) {
			projectile.move();
			if(projectile.isEmpty())emptyOnes.add(projectile);
		}
		
		//let them run
		for (Enemy enemy : enemies) {
			enemy.move();
			if(enemy.isDead())fallenOnes.add(enemy);
			if(enemy.isVictorious())victoriousOnes.add(enemy);
		}
		
		
		//dispose fallen
		for (Enemy enemy : fallenOnes) {
			enemies.remove(enemy);
			gold+=enemy.getGoldValue();
		}
		
		//dispose empty
		for (Projectile projectile : emptyOnes) {
			projectiles.remove(projectile);
		}
		
		//dispose victorious enemies
		for (Enemy enemy : victoriousOnes) {
			enemies.remove(enemy);
			hp-=enemy.getHPCost();
		}
		
		
	}


	private void startRount() {
		isRoundRunning = true;
		spawner.startSpawning();
		timer.start();
	}

	private void stopRound() {
		isRoundRunning = false;
		timer.stop();
	}




	private boolean isRoundOver() {
		
		return enemies.size()==0;
		
	};
	
	public boolean isGameOver(){
		return false;
	}
}
