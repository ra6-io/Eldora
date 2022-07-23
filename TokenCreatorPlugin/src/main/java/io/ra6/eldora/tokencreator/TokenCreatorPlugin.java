package io.ra6.eldora.tokencreator;

import io.ra6.eldora.AbstractEldoraPlugin;

public final class TokenCreatorPlugin extends AbstractEldoraPlugin {
	@Override
	public void onLoad() {
		addTabComponent(new TokenCreatorTab());
	}
}
