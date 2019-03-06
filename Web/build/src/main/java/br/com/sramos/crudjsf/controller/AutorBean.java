package com.frameweb.java;

import javax.faces.bean.ManagedBean;
import javax.faces.bean.SessionScoped;

@ManagedBean(name = "")
@SessionScoped
public class AutorBean extends AbstractController {

	private Autor autor;

	public Autor getAutor(){
		return autor;
	}

	public void setAutor(Autor _autor){
		autor = _autor;
	}

	private List autores;

	public List getAutores(){
		return autores;
	}

	public void setAutores(List _autores){
		autores = _autores;
	}

	private AutorBO autorBO;

	public AutorBO getAutorBO(){
		return autorBO;
	}

	public void setAutorBO(AutorBO _autorBO){
		autorBO = _autorBO;
	}



	public AutorBean(){
		
	}


	public void salvar(){
		
	}


	public void excluir(Autor autor){
		
	}


	
}