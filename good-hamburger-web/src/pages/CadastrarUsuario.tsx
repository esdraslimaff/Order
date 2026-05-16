import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { apiClient } from '../api/apiClient';

export const CadastrarUsuario: React.FC = () => {
  const navigate = useNavigate();
  
  // Usando um estado único para o formulário (parecido com o model do EditForm)
  const [novoUsuario, setNovoUsuario] = useState({
    nome: '',
    email: '',
    senha: '',
    perfil: 2 // 2 é o Enum para Atendente
  });

  const [salvando, setSalvando] = useState(false);
  const [mensagemErro, setMensagemErro] = useState('');
  const [mensagemSucesso, setMensagemSucesso] = useState('');

  const salvarUsuario = async (e: React.FormEvent) => {
    e.preventDefault();
    setSalvando(true);
    setMensagemErro('');
    setMensagemSucesso('');

    try {
      await apiClient.post('/Usuarios', novoUsuario);
      setMensagemSucesso('Usuário cadastrado com sucesso!');
      
      // Reseta o formulário
      setNovoUsuario({ nome: '', email: '', senha: '', perfil: 2 });
    } catch (error: any) {
      setMensagemErro('Erro ao cadastrar usuário. Verifique os dados e tente novamente.');
    } finally {
      setSalvando(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setNovoUsuario(prev => ({ 
      ...prev, 
      [name]: name === 'perfil' ? parseInt(value) : value 
    }));
  };

  return (
    <div className="container mt-4">
      <div className="card shadow-sm">
        <div className="card-header bg-primary text-white">
          <h4 className="mb-0">Cadastrar Novo Funcionário</h4>
        </div>
        <div className="card-body">
          
          {mensagemSucesso && <div className="alert alert-success">{mensagemSucesso}</div>}
          {mensagemErro && <div className="alert alert-danger">{mensagemErro}</div>}

          <form onSubmit={salvarUsuario}>
            <div className="row mb-3">
              <div className="col-md-6">
                <label className="form-label">Nome Completo</label>
                <input type="text" name="nome" className="form-control" value={novoUsuario.nome} onChange={handleChange} required />
              </div>
              <div className="col-md-6">
                <label className="form-label">E-mail</label>
                <input type="email" name="email" className="form-control" value={novoUsuario.email} onChange={handleChange} required />
              </div>
            </div>

            <div className="row mb-3">
              <div className="col-md-6">
                <label className="form-label">Senha Provisória</label>
                <input type="password" name="senha" className="form-control" value={novoUsuario.senha} onChange={handleChange} required />
              </div>
              <div className="col-md-6">
                <label className="form-label">Perfil de Acesso</label>
                <select name="perfil" className="form-select" value={novoUsuario.perfil} onChange={handleChange}>
                  {/* Simulando o foreach no Enum */}
                  <option value={1}>Admin</option>
                  <option value={2}>Atendente</option>
                </select>
              </div>
            </div>

            <div className="d-flex justify-content-end gap-2 mt-4">
              <button type="button" className="btn btn-secondary" onClick={() => navigate('/pedidos')}>Cancelar</button>
              <button type="submit" className="btn btn-success" disabled={salvando}>
                {salvando ? (
                  <><span className="spinner-border spinner-border-sm"></span> <span>Salvando...</span></>
                ) : (
                  <span>Cadastrar Usuário</span>
                )}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};