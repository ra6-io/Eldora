package io.ra6;

public class OSChecker {
	/**
	 * Operating systems.
	 */
	public enum OS {
		WINDOWS, LINUX, MAC, SOLARIS
	}

	private static OS os = null;

	/**
	 * Gets the operating system
	 *
	 * @return the current operating system
	 */
	public static OS getOS() {
		if (os == null) {
			var os = System.getProperty("os.name").toLowerCase();
			if (os.contains("win")) {
				OSChecker.os = OS.WINDOWS;
			} else if (os.contains("nix") || os.contains("nux") || os.contains("aix")) {
				OSChecker.os = OS.LINUX;
			} else if (os.contains("mac")) {
				OSChecker.os = OS.MAC;
			} else if (os.contains("sunos")) {
				OSChecker.os = OS.SOLARIS;
			}
		}
		return os;
	}
}