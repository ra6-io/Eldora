package io.ra6;

public final class Tuple<A, B> {
	private A _first;
	private B _second;

	public Tuple(A first, B second){
		_first = first;
		_second = second;
	}

	public A getFirst() {
		return _first;
	}

	public B getSecond() {
		return _second;
	}
}
