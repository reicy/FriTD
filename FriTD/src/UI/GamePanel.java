package UI;

import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.util.ArrayList;
import java.util.LinkedList;
import javax.swing.JPanel;

import Core.Application;

public class GamePanel extends JPanel{

	/*LinkedList<IDisplayableObject> enemies;
	LinkedList<IDisplayableObject> projectiles;
	IDisplayableObject[][] map;*/
	private Application app;
	private Dimension size = new Dimension(500,500);
	
	
    public GamePanel(Application app) {
		super();
		setSize(size);
	}

	@Override
    public void paint(Graphics g) {
		Graphics2D gr = (Graphics2D) g;
        gr.fillRect(0, 100, 200, 300);  
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
	
	


    
    
}
