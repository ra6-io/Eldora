package io.ra6.eldora.plugins;

import io.ra6.Eldora;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

public class PluginMethodHook<TReturnType> {
	private Method _hook;

	private final Object _instance;
	private final Class<TReturnType> _returnTypeClass;

	public PluginMethodHook(Object instance, Class<TReturnType> returnTypeClass) {
		_instance = instance;
		_returnTypeClass = returnTypeClass;
	}

	public void setHook(Method hook) {
		_hook = hook;
	}

	public void removeHook(Method hook) {
		_hook = null;
	}

	public TReturnType call(Object... args) throws InvocationTargetException, IllegalAccessException {
		if (_hook == null) {
			Eldora.LOGGER.warn("No hooks assigned to {}", _instance.getClass().getName());
			return null;
		}
		if (args.length != _hook.getParameterCount()) {
			Eldora.LOGGER.error("Mismatching parameters at function {}", _hook.getName());
		}
		return _returnTypeClass.cast(_hook.invoke(_instance, args));
	}
}
