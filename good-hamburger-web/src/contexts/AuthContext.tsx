import React, { createContext, useState, useEffect, type ReactNode } from 'react';

// Tipagem do usuário logado
export interface Usuario {
  email: string;
  role: string;
}

interface AuthContextType {
  usuario: Usuario | null;
  isAuthenticated: boolean;
  login: (token: string) => void;
  logout: () => void;
}

export const AuthContext = createContext<AuthContextType>({} as AuthContextType);

// Função simples para decodificar o payload do JWT (igual ao ParseBase64WithoutPadding)
const decodificarToken = (token: string): Usuario | null => {
  try {
    const payload = token.split('.')[1];
    const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      window.atob(base64).split('').map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join('')
    );
    const dados = JSON.parse(jsonPayload);
    
    // Mapeia os claims baseados no padrão do .NET
    const email = dados['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || dados.email;
    let role = dados['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || dados.role;
    
    // Conversão do Enum que você tinha no C#
    if (role === '1') role = 'Admin';
    if (role === '2') role = 'Atendente';

    return { email, role };
  } catch {
    return null;
  }
};

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [usuario, setUsuario] = useState<Usuario | null>(null);

  useEffect(() => {
    // Ao iniciar o app, verifica se já tem token salvo
    const tokenSalvo = localStorage.getItem('authToken');
    if (tokenSalvo) {
      setUsuario(decodificarToken(tokenSalvo));
    }
  }, []);

  const login = (token: string) => {
    localStorage.setItem('authToken', token);
    setUsuario(decodificarToken(token));
  };

  const logout = () => {
    localStorage.removeItem('authToken');
    setUsuario(null);
  };

  return (
    <AuthContext.Provider value={{ usuario, isAuthenticated: !!usuario, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};