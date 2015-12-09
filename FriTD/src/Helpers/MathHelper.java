package Helpers;

public class MathHelper {

	
	public static int getDistanceBetweenPoints(int x1, int y1, int x2, int y2){
		
		int diffX, diffY;
		diffX = x2-x1;
		diffY = y2-y1;
		
		return (int)Math.sqrt(diffX*diffX + diffY*diffY);
		
	}
	
}
