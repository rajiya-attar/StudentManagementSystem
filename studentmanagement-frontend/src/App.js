import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './components/Auth/Login';
import Register from './components/Auth/Register';
import StudentList from './components/Students/StudentList';
import StudentForm from './components/Students/StudentForm';
import Navbar from './components/Layout/Navbar';
import './App.css';

function App() {
  const [token, setToken] = useState(localStorage.getItem('token'));

  const handleLogin = (newToken) => {
    localStorage.setItem('token', newToken);
    setToken(newToken);
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    setToken(null);
  };

  return (
    <Router>
      <div className="App">
        {token && <Navbar onLogout={handleLogout} />}
        <div className="container">
          <Routes>
            <Route 
              path="/login" 
              element={!token ? <Login onLogin={handleLogin} /> : <Navigate to="/students" />} 
            />
            <Route 
              path="/register" 
              element={!token ? <Register /> : <Navigate to="/students" />} 
            />
            <Route 
              path="/students" 
              element={token ? <StudentList token={token} /> : <Navigate to="/login" />} 
            />
            <Route 
              path="/students/new" 
              element={token ? <StudentForm token={token} /> : <Navigate to="/login" />} 
            />
            <Route 
              path="/students/edit/:id" 
              element={token ? <StudentForm token={token} /> : <Navigate to="/login" />} 
            />
            <Route 
              path="/" 
              element={<Navigate to={token ? "/students" : "/login"} />} 
            />
          </Routes>
        </div>
      </div>
    </Router>
  );
}

export default App;
