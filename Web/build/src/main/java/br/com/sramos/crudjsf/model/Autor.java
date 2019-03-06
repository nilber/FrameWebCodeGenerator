package com.frameweb.java;


public class Autor  {



private String nome;

public String getNome(){
	return nome;
}

public void setNome(String _nome){
	nome = _nome;
}



@Column(name = "dataNascimento")
@Temporal(TemporalType.DATE)
private Date dataNascimento;

public Date getDataNascimento(){
	return dataNascimento;
}

public void setDataNascimento(Date _dataNascimento){
	dataNascimento = _dataNascimento;
}



private String email;

public String getEmail(){
	return email;
}

public void setEmail(String _email){
	email = _email;
}



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



	public Autor(){
		
	}


	
}