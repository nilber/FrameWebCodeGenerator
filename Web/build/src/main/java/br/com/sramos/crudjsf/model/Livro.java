package com.frameweb.java;


public class Livro  {



@Id
@GeneratedValue(strategy = GenerationType.IDENTITY)
@Column(name = "id")
private long id;

public long getId(){
	return id;
}

public void setId(long _id){
	id = _id;
}



private String nome;

public String getNome(){
	return nome;
}

public void setNome(String _nome){
	nome = _nome;
}



private String isbn;

public String getIsbn(){
	return isbn;
}

public void setIsbn(String _isbn){
	isbn = _isbn;
}



private boolean ativo;

public boolean getAtivo(){
	return ativo;
}

public void setAtivo(boolean _ativo){
	ativo = _ativo;
}



	public Livro(){
		
	}


	
}