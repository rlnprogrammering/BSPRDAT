package Assignment5;
import java.util.Arrays;


public class program {
	public static void main(String[] args) {
		int[] xs = {3, 5, 12};
		int[] ys = {2, 3, 4, 7};
		int[] res = merge(xs, ys);
		System.out.println(Arrays.toString(res));
	}

	/* Exercise 5.1 B */
	static int[] merge(int[] xs, int[] ys)
	{
		int[] res = new int[xs.length + ys.length];
		System.arraycopy(xs, 0, res, 0, xs.length);
		System.arraycopy(ys, 0, res, xs.length, ys.length);
		Arrays.sort(res);
		return res;
	}

}
