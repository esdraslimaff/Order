import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Navbar } from './components/Navbar';
import { Home } from './pages/Home';
import { Login } from './pages/Login';
import { NovoPedido } from './pages/NovoPedido';
import { Pedidos } from './pages/Pedidos';
import { ProtectedRoute } from './components/ProtectedRoute';
import { Detalhes } from './pages/Detalhes';
import { EditarPedido } from './pages/EditarPedido';
import { Promocoes } from './pages/Promocoes';
import { CadastrarUsuario } from './pages/CadastrarUsuario';

export const App: React.FC = () => {
  return (
    <Router>
      <div className="min-vh-100 bg-light">
        <Navbar />
        
        <main className="pb-5">
          <Routes>
            {/* Rotas Públicas */}
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />
            <Route path="/promocoes" element={<Promocoes />} />
            
            {/* Rotas Protegidas */}
            <Route path="/novo-pedido" element={
              <ProtectedRoute>
                <NovoPedido />
              </ProtectedRoute>
            } />
            
            <Route path="/pedidos" element={
              <ProtectedRoute>
                <Pedidos />
              </ProtectedRoute>
            } />

            <Route path="/pedidos/:id" element={
              <ProtectedRoute>
                <Detalhes />
              </ProtectedRoute>
            } />

            <Route path="/pedidos/editar/:id" element={
              <ProtectedRoute roles={['Admin']}>
                <EditarPedido />
              </ProtectedRoute>
            } />

            {/* ADICIONE A ROTA AQUI: */}
            <Route path="/usuarios/novo" element={
              <ProtectedRoute roles={['Admin']}>
                <CadastrarUsuario />
              </ProtectedRoute>
            } />
          </Routes>
        </main>
      </div>
    </Router>
  );
};

export default App;