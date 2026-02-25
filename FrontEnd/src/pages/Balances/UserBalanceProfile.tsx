import React, { useEffect, useState } from 'react';
import { Mail, User, Briefcase, Calendar } from 'lucide-react';
import type { UserWithDetail } from '@/pages/Users/UsersType';
import { useAuth } from '@/core/auth/AuthContext';
import { getUserById } from '@/pages/Users/UserApi';
import Loading from '@/components/common/Loading';
import LeaveBalancesDisplay from '@/pages/Users/LeaveBalancesDisplay';

const UserBalanceProfile: React.FC = () => {
  const { state } = useAuth();
  const [user, setUser] = useState<UserWithDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (state.user?.id) {
      fetchUserProfile();
    }
  }, [state.user?.id]);

  const fetchUserProfile = async () => {
    try {
      setLoading(true);
      const data = await getUserById(Number(state.user?.id));
      if (data) {
        setUser(data);
      }
    } catch (err) {
      setError("Failed to load profile");
      console.error("Fetch user profile failed", err);
    } finally {
      setLoading(false);
    }
  };

  const getAvatarUrl = () => {
    if (user?.detail?.UserPhoto) {
      return `data:image/jpeg;base64,${user.detail.UserPhoto}`;
    }
    return `https://api.dicebear.com/7.x/avataaars/svg?seed=${user?.detail?.FullName || user?.userID}`;
  };

  if (loading) return <Loading />;

  if (!user) {
    return (
      <div className="text-center py-20 text-gray-500 font-medium">
        Unable to load profile. Please try again later.
      </div>
    );
  }

  return (
    <div className="space-y-8 animate-in fade-in duration-500">
      {/* Error Message Toast */}
      {error && (
        <div className="fixed top-4 right-4 z-50 bg-red-500 text-white px-6 py-3 rounded-xl shadow-lg">
          <div className="flex items-center gap-3">
            <span>{error}</span>
            <button onClick={() => setError(null)} className="text-white hover:text-red-100">×</button>
          </div>
        </div>
      )}

      {/* Header */}
      <div className="flex flex-col lg:flex-row justify-between items-start lg:items-center gap-6 bg-white p-8 rounded-3xl shadow-sm border border-gray-50">
        <div>
          <h1 className="text-3xl font-black text-gray-900 tracking-tight">My Leave Balances</h1>
          <p className="text-gray-500 font-medium">View your available leave balances.</p>
        </div>
      </div>

      {/* User Info Card */}
      <div className="bg-white rounded-3xl shadow-sm border border-gray-50 overflow-hidden">
        {/* User Header */}
        <div className="bg-gradient-to-r from-emerald-500 to-teal-600 px-8 py-8">
          <div className="flex flex-col md:flex-row items-center gap-6">
            <img
              src={getAvatarUrl()}
              className="w-20 h-20 rounded-2xl shadow-xl border-4 border-white object-cover"
              alt={`${user.firstName} ${user.lastName}`}
            />
            <div className="text-center md:text-left">
              <h2 className="text-2xl font-black text-white">
                {user.firstName} {user.lastName}
              </h2>
              <p className="text-emerald-100 font-medium mt-1">{user.email}</p>
            </div>
          </div>
        </div>

        {/* Quick Info */}
        <div className="p-6 border-b border-gray-100">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-xl">
              <div className="w-10 h-10 rounded-lg bg-blue-100 flex items-center justify-center">
                <User size={18} className="text-blue-600" />
              </div>
              <div>
                <p className="text-[10px] font-black text-gray-400 uppercase tracking-widest">Username</p>
                <p className="font-bold text-gray-900 text-sm">{user.userName}</p>
              </div>
            </div>

            <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-xl">
              <div className="w-10 h-10 rounded-lg bg-green-100 flex items-center justify-center">
                <Mail size={18} className="text-green-600" />
              </div>
              <div>
                <p className="text-[10px] font-black text-gray-400 uppercase tracking-widest">Email</p>
                <p className="font-bold text-gray-900 text-sm truncate">{user.email || 'N/A'}</p>
              </div>
            </div>

            <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-xl">
              <div className="w-10 h-10 rounded-lg bg-indigo-100 flex items-center justify-center">
                <Calendar size={18} className="text-indigo-600" />
              </div>
              <div>
                <p className="text-[10px] font-black text-gray-400 uppercase tracking-widest">Current Year</p>
                <p className="font-bold text-gray-900 text-sm">{new Date().getFullYear()}</p>
              </div>
            </div>
          </div>
        </div>

        {/* Leave Balances Section - Primary Focus */}
        <div className="p-8">
          <div className="flex items-center gap-3 mb-6">
            <div className="w-12 h-12 rounded-xl bg-emerald-100 flex items-center justify-center">
              <Briefcase size={24} className="text-emerald-600" />
            </div>
            <div>
              <h3 className="text-xl font-black text-gray-900">Leave Balances</h3>
              <p className="text-sm text-gray-500">Your available leave days by type</p>
            </div>
          </div>
          
          {user?.leaveBalances && user.leaveBalances.length > 0 ? (
            <LeaveBalancesDisplay leaveBalances={user.leaveBalances} variant="card" />
          ) : (
            <div className="text-center py-12 bg-gray-50 rounded-2xl">
              <div className="w-16 h-16 rounded-2xl bg-gray-100 flex items-center justify-center mx-auto mb-4">
                <Briefcase size={32} className="text-gray-400" />
              </div>
              <p className="text-gray-500 font-medium">No leave balances available</p>
              <p className="text-sm text-gray-400 mt-1">Contact your administrator to set up your leave balances.</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

export default UserBalanceProfile;
