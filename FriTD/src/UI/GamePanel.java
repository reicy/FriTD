package UI;

import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.ArrayList;
import java.util.LinkedList;

import javax.swing.JButton;
import javax.swing.JPanel;
import javax.swing.Timer;

import Core.Application;
import Enums.MapSquareType;

public class GamePanel extends JPanel implements ActionListener{

	/*LinkedList<IDisplayableObject> enemies;
	LinkedList<IDisplayableObject> projectiles;
	IDisplayableObject[][] map;*/
	private Application app;
	private Dimension size = new Dimension(500,500);
	private Dimension gameBoardSize;
	private IDisplayableObject[][] map;
	private JButton addTowerButton;
	private Timer timer;
	
    public GamePanel(Application app) {
    	
		super();
		timer = new Timer(100,this);
		setSize(size);
		this.app=app;
		map=app.getMap();
		gameBoardSize = new Dimension(map.length*app.getMapSquareSize(),map.length*app.getMapSquareSize());
		//addTowerButton = new JButton("Add tower!");
		//addTowerButton.addActionListener(new MyAction());
		//this.add(addTowerButton);
		timer.start();
	}

	@Override
    public void paint(Graphics g) {
		app.start();
		Graphics2D g2d = (Graphics2D) g;
        //gr.fillRect(0, 100, 200, 300);  
		//System.out.println(app.getMap());
		for(IDisplayableObject[] row : app.getMap()){
			for(IDisplayableObject elem : row){			
				elem.display(g2d);
			}
		}
		
		for(IDisplayableObject en : app.getEnemies()){
			en.display(g2d);
		}
		
		/*for(IDisplayableObject p : app.getProjectiles()){
			p.display(g2d);
		}*/
		
		g2d.drawString("TOWER DEFENCE",(int)gameBoardSize.getWidth(), (int)gameBoardSize.getHeight());
		
    }

	public Application getApp() {
		return app;
	}

	public void setApp(Application app) {
		this.app = app;
	}

	public Dimension getSize() {
		return size;
	}

	public void setSize(Dimension size) {
		this.size = size;
	}

	@Override
	public void actionPerformed(ActionEvent e) {
		this.repaint();
		
	}
	
	


    
    
}
