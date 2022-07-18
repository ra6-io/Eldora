package io.ra6;

public final class Version {
	private final int _major;
	private final int _minor;
	private final int _patch;

	public Version(int major, int minor, int patch) {
		_major = major;
		_minor = minor;
		_patch = patch;
	}

	public Version(int major, int minor) {
		this(major, minor, 0);
	}

	public Version(int major) {
		this(major, 0);
	}

	public Version(String pluginVersion) {
		if (pluginVersion.startsWith("v") || pluginVersion.startsWith("V")) {
			pluginVersion = pluginVersion.substring(1);
		}

		var numbers = pluginVersion.split("\\.");
		if (numbers.length != 3) {
			_major = 0;
			_minor = 0;
			_patch = 0;
			return;
		}

		_major = Integer.parseInt(numbers[0]);
		_minor = Integer.parseInt(numbers[1]);
		_patch = Integer.parseInt(numbers[2]);
	}

	public int getMajor() {
		return _major;
	}

	public int getMinor() {
		return _minor;
	}

	public int getPatch() {
		return _patch;
	}

	@Override
	public String toString() {
		return "v%d.%d.%d".formatted(_major, _minor, _patch);
	}

	/**
	 * Returns true if the other version is greater or equal to this version
	 *
	 * @param other The other version to be checked against
	 * @return True if the other version is greater or equal to this version
	 */
	public boolean isGreaterOrEqualThan(Version other) {
		if (other._major >= _major) return true;
		if (other._minor >= _minor) return true;
		if (other._patch >= _patch) return true;
		return false;
	}
}
