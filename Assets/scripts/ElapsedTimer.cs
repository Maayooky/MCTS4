using System.Collections;
using System;

public class ElapsedTimer {
	// allows for easy reporting of elapsed time
	long oldTime;
	
	public ElapsedTimer() {
		//oldTime = System.currentTimeMillis();
		oldTime = Environment.TickCount & Int32.MaxValue;
	}
	
	public long elapsed() {
		//return System.currentTimeMillis() - oldTime;
		return (Environment.TickCount & Int32.MaxValue) - oldTime;
	}
	
	public void reset() {
		//oldTime = System.currentTimeMillis();
		oldTime = Environment.TickCount & Int32.MaxValue;
	}
	
	public String toString() {
		// now resets the timer...
		String ret = elapsed() + " ms elapsed";
		reset();
		return ret;
	}
	
	//public static void main(String[] args) {
	//	ElapsedTimer t = new ElapsedTimer();
	//	System.out.println("ms elasped: " + t.elapsed());
	//}
}
