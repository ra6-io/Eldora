package io.ra6.eldora.tokencreator;

import org.jetbrains.annotations.NotNull;

import java.awt.*;

public class Utils {

	/**
	 * Equal to {@code a} - {@code b}
	 * @param a
	 * @param b
	 * @return
	 */
	@NotNull
	public static Point getDifference(@NotNull Point a, @NotNull Point b) {
		return new Point(a.x - b.x, a.y - b.y);
	}
}
