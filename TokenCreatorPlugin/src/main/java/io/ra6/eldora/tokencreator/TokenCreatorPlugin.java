package io.ra6.eldora.tokencreator;

import io.ra6.eldora.AbstractEldoraPlugin;
import org.apache.logging.log4j.core.util.FileUtils;
import org.apache.logging.log4j.core.util.IOUtils;
import org.json.JSONObject;

import java.io.*;
import java.nio.file.CopyOption;
import java.nio.file.Files;
import java.nio.file.StandardCopyOption;
import java.util.ArrayList;

public final class TokenCreatorPlugin extends AbstractEldoraPlugin {
	private static TokenCreatorPlugin INSTANCE;

	public static TokenCreatorPlugin getInstance() {
		return INSTANCE;
	}


	private final ArrayList<String> _defaultTokens = new ArrayList<>() {
		{
			add("10_sided_ring_1");
			add("12_sided_ring_1");
			add("abstract_double_ring_1");
			add("abstract_double_ring_1_flat");
			add("abstract_octagon_1");
			add("abstract_octagon_1_flat");
			add("abstract_ring_1");
			add("abstract_ring_1_flat");
			add("abstract_ring_2");
			add("abstract_ring_2_flat");
			add("abstract_ring_3");
			add("abstract_ring_3_flat");
			add("abstract_triangle_1");
			add("cloud_ring_1");
			add("double_ring_1");
			add("fifthed_border_medium");
			add("fifthed_border_medium_greyscale");
			add("plain_hexagon_1");
			add("plain_octagon_1");
			add("plain_ring_1");
			add("plain_ring_1_with_arrow_1");
			add("plain_ring_2");
			add("plain_ring_3");
			add("plain_square_1");
			add("plain_square_2");
			add("spike_ring_1");
			add("spike_ring_2");
			add("wavey_ring_1");
		}
	};
	private final ArrayList<String> _defaultMasks = new ArrayList<>() {
		{
			add("10_sided_ring_mask");
			add("12_sided_ring_mask");
			add("abstract_triangle_mask");
			add("cloud_ring_1_mask");
			add("plain_hexagon_mask");
			add("plain_octagon_mask");
			add("plain_ring_mask");
			add("plain_square_mask");
			add("wavey_ring_1_mask");
		}
	};

	private final ArrayList<BorderObject> _loadedBorders = new ArrayList<>();

	private String _pluginPath = "";
	private String _borderPath = "";

	@Override
	public void onLoad(String pluginPath) {
		INSTANCE = this;
		_pluginPath = pluginPath;
		_borderPath = _pluginPath + "/borders";

		addTabComponent(new TokenCreatorTab());

		exportDefaultBorders();
	}

	public String getBorderPath() {
		return _borderPath;
	}

	@Override
	public void onEnable() {
		loadBorders();
	}

	public BorderObject getBorderFromName(String name) {
		for (BorderObject borderObject : _loadedBorders) {
			if (borderObject.getName().equals(name)) {
				return borderObject;
			}
		}
		return null;
	}

	public ArrayList<BorderObject> getLoadedBorders() {
		return _loadedBorders;
	}

	private void loadBorders() {
		var files = new File(_borderPath).listFiles((dir, name) -> name.endsWith(".json"));

		if (files == null) return;

		for (var file : files) {
			try {
				loadBorder(file);
			} catch (IOException e) {
				getLogger().info(e);
			}
		}
	}

	private void loadBorder(File file) throws IOException {
		getLogger().info("Loading border {}", file.getName());

		var reader = new BufferedReader(new FileReader(file));
		var result = new StringBuilder();

		String line;
		while ((line = reader.readLine()) != null) {
			result.append(line);
		}
		reader.close();

		var plugin = new JSONObject(result.substring(result.indexOf("{")));
		if (!plugin.has("name")) {
			getLogger().error("Missing key {} on {}", "name", file.getName());
			return;
		}

		if (!plugin.has("image")) {
			getLogger().error("Missing key {} on {}", "image", file.getName());
			return;
		}

		if (!plugin.has("mask")) {
			getLogger().error("Missing key {} on {}", "mask", file.getName());
			return;
		}

		var border = new BorderObject(plugin.getString("name"), plugin.getString("image"), plugin.getString("mask"));
		_loadedBorders.add(border);
	}

	private void exportDefaultBorders() {
		_defaultTokens.forEach(tokenName -> {
			var pngPath = "borders/" + tokenName + ".png";
			var jsonPath = "borders/" + tokenName + ".json";
			export(pngPath, _pluginPath + "/" + pngPath);
			export(jsonPath, _pluginPath + "/" + jsonPath);

		});

		_defaultMasks.forEach(maskName -> {
			var path = "borders/" + maskName + ".png";
			export(path, _pluginPath + "/" + path);
		});
	}

	private void export(String internalPath, String filePath) {
		try (var stream = Thread.currentThread().getContextClassLoader().getResourceAsStream(internalPath)) {
			if (stream == null) return;

			var file = new File(filePath);
			if (!file.exists()) {
				//noinspection ResultOfMethodCallIgnored
				file.getParentFile().mkdir();
				getLogger().info("Copying {} to {}", internalPath, filePath);
				if (!file.createNewFile()) {
					getLogger().error("Could not create {}", filePath);
				}
			} else {
				getLogger().info("{} already exists. SKIPPING", filePath);
				return;
			}

			Files.copy(stream, file.toPath(), StandardCopyOption.REPLACE_EXISTING);
		} catch (IOException e) {
			throw new RuntimeException(e);
		}
	}
}
