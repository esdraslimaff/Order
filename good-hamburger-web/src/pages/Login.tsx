import React, { useState, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { apiClient } from '../api/apiClient';
import { AuthContext } from '../contexts/AuthContext';

export const Login: React.FC = () => {
  const navigate = useNavigate();
  const { login } = useContext(AuthContext);
  
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [mensagemErro, setMensagemErro] = useState('');
  const [carregando, setCarregando] = useState(false);

  const fazerLogin = async (e: React.FormEvent) => {
    e.preventDefault(); // Evita recarregar a página
    setCarregando(true);
    setMensagemErro('');

    try {
      const response = await apiClient.post('/Auth/login', { email, senha });
      
      if (response.data && response.data.token) {
        login(response.data.token); // Salva o token no Contexto e no LocalStorage
        navigate('/novo-pedido');   // Redireciona
      }
    } catch (error) {
      setMensagemErro('E-mail ou senha incorretos.');
    } finally {
      setCarregando(false);
    }
  };

  return (
    <div className="container mt-5">
      <div className="row justify-content-center">
        <div className="col-md-5">
          <div className="card shadow">
            <div className="card-header bg-dark text-white text-center">
              <h4 className="mb-0">Área Restrita 🍔</h4>
            </div>
            <div className="card-body p-4">
              
              {mensagemErro && (
                <div className="alert alert-danger p-2 small">
                  {mensagemErro}
                </div>
              )}

              <form onSubmit={fazerLogin}>
                <div className="mb-3">
                  <label className="form-label">E-mail</label>
                  <input 
                    type="email" 
                    className="form-control" 
                    placeholder="admin@goodhamburger.com"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                  />
                </div>

                <div className="mb-4">
                  <label className="form-label">Senha</label>
                  <input 
                    type="password" 
                    className="form-control" 
                    placeholder="******"
                    value={senha}
                    onChange={(e) => setSenha(e.target.value)}
                    required
                  />
                </div>

                <button type="submit" className="btn btn-primary w-100 fw-bold" disabled={carregando}>
                  {carregando ? (
                    <>
                      <span className="spinner-border spinner-border-sm me-2"></span>
                      Entrando...
                    </>
                  ) : (
                    'Entrar'
                  )}
                </button>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};