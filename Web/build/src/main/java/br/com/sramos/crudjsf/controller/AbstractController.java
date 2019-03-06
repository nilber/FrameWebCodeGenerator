package com.frameweb.java;

import javax.faces.bean.ManagedBean;
import javax.faces.bean.SessionScoped;

@ManagedBean(name = "")
@SessionScoped
public abstract class AbstractController  {



	public AbstractController(){
		
	}


	public void mensagemInformacao(String titulo, String descricao){
		
	}


	public void mensagemAlerta(String titulo, String descricao){
		
	}


	public void mensagemErro(String titulo, String descricao){
		
	}


	public void mensagemErroFatal(Exception e, String titulo, String descricao){
		
	}


	
}