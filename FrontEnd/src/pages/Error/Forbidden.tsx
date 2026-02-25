import { useAuth } from '@/core/auth/AuthContext';
import { ShieldX } from 'lucide-react';
import { Link } from 'react-router-dom';

export default function Forbidden() {
    const { state } = useAuth();

    const handleGoToHome = () => {
        console.log("User is authenticated:", state.isAuthenticated);
        if(state.isAuthenticated)
            return "/requests";
        else
            return "/login"; 
    }

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-50">
            <div className="text-center p-8">
                <div className="flex justify-center mb-6">
                    <div className="w-20 h-20 bg-red-100 rounded-full flex items-center justify-center">
                        <ShieldX size={40} className="text-red-500" />
                    </div>
                </div>
                <h1 className="text-6xl font-bold text-gray-800 mb-2">403</h1>
                <h2 className="text-2xl font-semibold text-gray-700 mb-4">Access Denied</h2>
                <p className="text-gray-500 mb-8 max-w-md">
                    You don't have permission to access this page. Please contact your administrator if you believe this is a mistake.
                </p>
                <Link
                    to={handleGoToHome()}
                    className="inline-flex items-center gap-2 px-6 py-3 bg-blue-600 text-white font-semibold rounded-xl hover:bg-blue-700 transition-colors shadow-lg shadow-blue-200"
                >
                    Go to Home
                </Link>
            </div>
        </div>
    );
}
