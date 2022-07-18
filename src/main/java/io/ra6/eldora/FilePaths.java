package io.ra6.eldora;

import io.ra6.Eldora;

import java.io.File;

public final class FilePaths {
    public static final File RootPath = new File(System.getenv("APPDATA"), "Eldora");
    public static final File PluginPath = new File(RootPath, "plugins");
    public static final File LanguagePath = new File(RootPath, "lang");
    public static final File UpdaterPath = new File(RootPath, "updater");
    public static final File LogPath = new File(RootPath, "logs");
    public static final File SettingsPath = new File(RootPath, "settings");

    /**
     * Creates a folder if it does not exist
     * @param path The folder which to be created
     */
    private static void createFolderIfNotExist(File path) {
        if (!path.exists()) {
            Eldora.LOGGER.info("Creating folder {}", path.getAbsolutePath());
            if (!path.mkdir()) {
                Eldora.LOGGER.warn("Could not create folder {}", path.getAbsolutePath());
            }
        } else {
            Eldora.LOGGER.info("Folder {} already exists. SKIPPING", path.getAbsolutePath());
        }
    }

    /**
     * Creates the basic folder structure for the application
     */
    public static void createFolderStructure() {
        Eldora.LOGGER.info("Creating folders");

        createFolderIfNotExist(RootPath);
        createFolderIfNotExist(PluginPath);
        createFolderIfNotExist(LanguagePath);
        createFolderIfNotExist(UpdaterPath);
        createFolderIfNotExist(LogPath);
        createFolderIfNotExist(SettingsPath);

        Eldora.LOGGER.info("Finished Creating folders");
    }

}
