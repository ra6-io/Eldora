package io.ra6.eldora.plugins;

import io.ra6.Version;

import java.util.Arrays;

public class PluginMetadata {
	private final String _name;
	private final String _package;
	private final String[] _author;
	private final Version _version;
	private final String _description;
	private final String _url;

	public PluginMetadata(String name, String aPackage, String[] author, Version version, String description, String url) {
		_name = name;
		_package = aPackage;
		_author = author;
		_version = version;
		_description = description;
		_url = url;
	}

	public String getName() {
		return _name;
	}

	public String getPackage() {
		return _package;
	}

	public String[] getAuthor() {
		return _author;
	}

	public Version getVersion() {
		return _version;
	}

	public String getDescription() {
		return _description;
	}

	public String getUrl() {
		return _url;
	}

	@Override
	public String toString() {
		return "PluginMetadata{" +
				"_name='" + _name + '\'' +
				", _package='" + _package + '\'' +
				", _author=" + Arrays.toString(_author) +
				", _version=" + _version +
				", _description='" + _description + '\'' +
				", _url='" + _url + '\'' +
				'}';
	}
}
