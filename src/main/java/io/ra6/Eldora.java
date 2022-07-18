package io.ra6;

import io.ra6.eldora.FilePaths;
import io.ra6.eldora.plugins.PluginHandler;
import io.ra6.eldora.ui.MainFrame;
import org.apache.logging.log4j.Level;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.apache.logging.log4j.core.appender.ConsoleAppender;
import org.apache.logging.log4j.core.config.Configurator;
import org.apache.logging.log4j.core.config.builder.api.ConfigurationBuilderFactory;

import javax.swing.*;

public class Eldora {

	/**
	 * The default logger for the application
	 */
	public static final Logger LOGGER = LogManager.getLogger();

	/**
	 * The Application version
	 */
	public static final Version VERSION = new Version(0, 0, 1);

	/**
	 * The title of eldora
	 */
	public static final String TITLE = "Eldora - " + VERSION;

	/**
	 * The default instance of eldora
	 */
	private static Eldora INSTANCE;


	private PluginHandler _pluginHandler;
	private MainFrame _mainFrame;

	public static Eldora getInstance() {
		if (INSTANCE == null) {
			LOGGER.error("Instance is not set!");
		}
		return INSTANCE;
	}

	/**
	 * Initializes the application
	 */
	public static void initialize() {
		if (INSTANCE != null) {
			LOGGER.error("Eldora already initialized");
			return;
		}

		INSTANCE = new Eldora();

		initializeLogger();
		logSystemInformation();

		LOGGER.info("--Starting Eldora--");

		FilePaths.createFolderStructure();

		LOGGER.info("Adding shutdown hooks");
		Runtime.getRuntime().addShutdownHook(new Thread(Eldora::shutdown));

		INSTANCE._pluginHandler = new PluginHandler();
		INSTANCE._pluginHandler.loadPlugins(FilePaths.PluginPath);

		LOGGER.info("Loading settings");
		loadSettings();

		LOGGER.info("Starting UI");
		startupUi();
		LOGGER.info("--Finished starting--");
	}

	private static void loadSettings() {
	}

	private static void shutdown() {
		LOGGER.info("Shutting down Eldora");

	}

	private static void startupUi() {
		// Sets the look and feel of the app
		try {
			var systemLAF = UIManager.getSystemLookAndFeelClassName();
			LOGGER.info("Setting look and feel to {}", systemLAF);
			UIManager.setLookAndFeel(systemLAF);
		} catch (ClassNotFoundException | InstantiationException | IllegalAccessException |
		         UnsupportedLookAndFeelException e) {
			LOGGER.fatal("Could not set the look and feel.", e);
			throw new RuntimeException(e);
		}

		SwingUtilities.invokeLater(() -> {
			getInstance()._mainFrame = new MainFrame();
			getInstance()._mainFrame.initialize();
		});
	}

	/**
	 * Logs the system information
	 */
	private static void logSystemInformation() {
		LOGGER.info("System info OS Name    {}", System.getProperty("os.name"));
		LOGGER.info("            OS Version {}", System.getProperty("os.version"));
		LOGGER.info("            OS Arch    {}", System.getProperty("os.arch"));

		LOGGER.info("            JDK        {}", System.getProperty("java.version"));
		LOGGER.info("            JRE        {}", System.getProperty("java.runtime.version"));

		LOGGER.info("            Eldora     {}", VERSION);
	}

	/**
	 * Initializes the logger
	 */
	private static void initializeLogger() {
		var logFileName = FilePaths.LogPath.getAbsolutePath();
		var pattern = "[%d] - [%t] %p %C : %m%n";

		// create default logger
		var builder = ConfigurationBuilderFactory.newConfigurationBuilder();
		builder.setStatusLevel(Level.INFO);
		builder.setConfigurationName("DefaultLogger");

		//console appender
		var consoleAppenderBuilder = builder.newAppender("Console", "CONSOLE")
				.addAttribute("target", ConsoleAppender.Target.SYSTEM_OUT);
		consoleAppenderBuilder.add(builder.newLayout("PatternLayout")
				.addAttribute("pattern", pattern));

		var patternLayout = builder.newLayout("PatternLayout")
				.addAttribute("pattern", pattern);
		var triggeringPolicy = builder.newComponent("Policies")
				.addComponent(builder.newComponent("SizeBasedTriggeringPolicy").addAttribute("size", "5MB"));

		// create a rolling appender
		var rollingFileAppenderBuilder = builder.newAppender("LogToRollingFile", "RollingFile")
				.addAttribute("fileName", logFileName + "\\log.log")
				.addAttribute("filePattern", logFileName + "\\%d{yyyy-MM-dd_HH-mm-ss}.log.dat")
				.add(patternLayout)
				.addComponent(triggeringPolicy);

		var rootLogger = builder.newRootLogger(Level.DEBUG);
		rootLogger.add(builder.newAppenderRef("Console"));
		rootLogger.add(builder.newAppenderRef("LogToRollingFile"));

		builder.add(consoleAppenderBuilder);
		builder.add(rollingFileAppenderBuilder);

		builder.add(rootLogger);

		Configurator.reconfigure(builder.build());
	}
}
