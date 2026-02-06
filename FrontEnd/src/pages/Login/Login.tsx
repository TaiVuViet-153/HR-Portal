
import React, { useState } from 'react';
import { ShieldCheck, ShieldUser, Lock, LogIn, Sparkles } from 'lucide-react';
// import { UserRole, type UserRoleValue } from '@/Core/Auth/AuthType';
import { useAuth } from '@/core/auth/AuthContext';
import { useNavigate } from 'react-router-dom';
import { loginApi } from '@/pages/Login/LoginApi';


export function Login() {
  const navigate = useNavigate();
  const { login } = useAuth();

  // Form state
  const [username, setUsername] = useState<string>('');
  const [password, setPassword] = useState<string>('');
  // const [email, setEmail] = useState<string>(''); // dùng khi Sign Up
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      const { accessToken, user, isRequireChangedPW } = await loginApi(username, password);

      login({ accessToken, user });
      
      if (isRequireChangedPW) {
        navigate('/change-password', { replace: true });
      } else {
        navigate('/', { replace: true });
      }
    } catch (err: any) {
      const message =
        err?.response?.data?.message ||
        err?.message ||
        'Đăng nhập thất bại. Vui lòng thử lại.';
      setError(message);
      console.error(err);
    } finally {
      setLoading(false);
    }

  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 p-6">
      <div className="max-w-md w-full">
        <div className="text-center mb-10">
          <div className="w-16 h-16 bg-blue-600 rounded-2xl mx-auto flex items-center justify-center text-white mb-4 shadow-xl">
            <ShieldCheck size={32} />
          </div>
          <h1 className="text-3xl font-bold text-gray-900">Linh Long HRM</h1>
          <p className="text-gray-500 mt-2">Intelligent Workforce Management</p>
        </div>

        <div className="bg-white p-8 rounded-3xl shadow-xl border border-gray-100">
          <form onSubmit={handleSubmit} className="space-y-6">
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-2">Username</label>
              <div className="relative">
                <ShieldUser className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" size={18} />
                <input 
                  type="text" 
                  className="w-full pl-10 pr-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:bg-white focus:outline-none transition-all"
                  value={username}
                  onChange={e => setUsername(e.target.value)}
                  required
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-2">Password</label>
              <div className="relative">
                <Lock className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" size={18} />
                <input 
                  type="password" 
                  className="w-full pl-10 pr-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:bg-white focus:outline-none transition-all"
                  value={password}
                  onChange={e => setPassword(e.target.value)}
                  required
                />
              </div>
            </div>

            <button 
              type="submit" 
              className="w-full py-4 bg-blue-600 text-white rounded-2xl font-bold text-lg hover:bg-blue-700 transition-all flex items-center justify-center gap-2 shadow-lg shadow-blue-200 active:scale-[0.98] cursor-pointer"
              disabled={loading}
            >
              <LogIn size={20} />
               {loading ? 'Đang đăng nhập...' : 'Sign In'}
            </button>

            {error && 
              <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded relative" role="alert">
                <strong className="font-bold">Error! </strong>
                <span className="block sm:inline">{error}</span>
                <span className="absolute top-0 bottom-0 right-0 px-4 py-3">
                  <svg className="fill-current h-6 w-6 text-red-500" role="button" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"><title>Close</title><path d="M14.348 14.849a1.2 1.2 0 0 1-1.697 0L10 11.819l-2.651 3.029a1.2 1.2 0 1 1-1.697-1.697l2.758-3.15-2.759-3.152a1.2 1.2 0 1 1 1.697-1.697L10 8.183l2.651-3.031a1.2 1.2 0 1 1 1.697 1.697l-2.758 3.152 2.758 3.15a1.2 1.2 0 0 1 0 1.698z"/></svg>
                </span>
              </div>
            }
          </form>

          <div className="mt-8 pt-6 border-t border-gray-100">
            <div className="flex items-center gap-3 p-4 bg-indigo-50 rounded-2xl text-indigo-700">
              <Sparkles size={20} className="flex-shrink-0" />
              <p className="text-xs leading-relaxed">
                Experience AI-driven insights and automated workflows with Linh Long HRM.
              </p>
            </div>
          </div>
        </div>
        
        <p className="text-center text-gray-400 text-sm mt-8">
          &copy; 2026 Linh Long Solutions Inc. All rights reserved.
        </p>
      </div>
    </div>
  );
};
