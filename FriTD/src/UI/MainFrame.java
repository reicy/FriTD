package UI;

import javax.swing.JOptionPane;

import Core.Application;

public class MainFrame extends javax.swing.JFrame {

	private GamePanel gamePanel;
	
    public MainFrame(GamePanel gamePanel) {
        this.gamePanel=gamePanel;
    	this.setVisible(false);
        this.add(gamePanel);
        setSize(gamePanel.getSize());
    }

    

    /**
     * @param args the command line arguments
     */
    public static void main(String args[]) {
       
    	
    	
    	
    	
        java.awt.EventQueue.invokeLater(new Runnable() {
            public void run() {
                new MainFrame(new GamePanel(new Application(Double.parseDouble(JOptionPane.showInputDialog(null, "Speed modifier (double value - smaller is faster)"))))).setVisible(true);
            }
        });
    }
}