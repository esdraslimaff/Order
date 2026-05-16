import React, { useContext } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { AuthContext } from '../contexts/AuthContext';

export const Navbar: React.FC = () => {
  const navigate = useNavigate();
  const { isAuthenticated, usuario, logout } = useContext(AuthContext);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark shadow-sm">
      <div className="container">
        <Link className="navbar-brand fw-bold" to="/">🍔 Good Hamburger</Link>
        
        <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
          <span className="navbar-toggler-icon"></span>
        </button>
        
        <div className="collapse navbar-collapse" id="navbarNav">
          <ul className="navbar-nav me-auto">
            <li className="nav-item">
              <Link className="nav-link" to="/">Cardápio</Link>
            </li>
            <li className="nav-item">
                <Link className="nav-link" to="/promocoes">Promoções</Link>
                </li>
            {isAuthenticated && (
              <>
                <li className="nav-item">
                  <Link className="nav-link" to="/novo-pedido">Novo Pedido</Link>
                </li>
                <li className="nav-item">
                  <Link className="nav-link" to="/pedidos">Histórico</Link>
                </li>
                {usuario?.role === 'Admin' && (
                <li className="nav-item">
                    <Link className="nav-link" to="/usuarios/novo">Novo Usuário</Link>
                </li>
                )}
              </>
            )}
          </ul>
          
          <div className="d-flex align-items-center">
            {isAuthenticated ? (
              <>
                <span className="text-light me-3 small">Olá, {usuario?.email}</span>
                <button className="btn btn-outline-light btn-sm" onClick={handleLogout}>Sair</button>
              </>
            ) : (
              <Link className="btn className-primary btn-sm" to="/login">Entrar</Link>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
};