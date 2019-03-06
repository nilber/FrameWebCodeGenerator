package com.frameweb.java;

import javax.faces.bean.ManagedBean;
import javax.faces.bean.SessionScoped;

@ManagedBean(name = "")
@SessionScoped
public class LivroBean extends AbstractController {

	private LivroBO livroBO;

	public LivroBO getLivroBO(){
		return livroBO;
	}

	public void setLivroBO(LivroBO _livroBO){
		livroBO = _livroBO;
	}

	private AutorBO autorBO;

	public AutorBO getAutorBO(){
		return autorBO;
	}

	public void setAutorBO(AutorBO _autorBO){
		autorBO = _autorBO;
	}

	private Livro livro;

	public Livro getLivro(){
		return livro;
	}

	public void setLivro(Livro _livro){
		livro = _livro;
	}

	private List livros;

	public List getLivros(){
		return livros;
	}

	public void setLivros(List _livros){
		livros = _livros;
	}

	private List autores;

	public List getAutores(){
		return autores;
	}

	public void setAutores(List _autores){
		autores = _autores;
	}



	public LivroBean(){
		
	}


	public void salvar(){
		
	}


	public void editar(Livro livro){
		
	}


	public void excluir(Livro livro){
		
	}


	public void autoresTransferidos(){
		
	}


	
}