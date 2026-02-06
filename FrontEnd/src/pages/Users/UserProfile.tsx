import React, { useEffect, useState } from 'react';
import { Edit2, Lock, Mail, Phone, MapPin, Calendar, User, Briefcase } from 'lucide-react';
import type { UserWithDetail } from '@/pages/Users/UsersType';
import { useAuth } from '@/core/auth/AuthContext';
import { getUserById, updateUser } from './UserApi';
import Loading from '@/components/common/Loading';
import UserFormModal from './UserFormModal';
import ChangePasswordModal from './ChangePasswordModal';
import { UserStatus } from './UsersData';
import LeaveBalancesDisplay from './LeaveBalancesDisplay';

const UserProfile: React.FC = () => {
  const { state } = useAuth();
  const [user, setUser] = useState<UserWithDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [modalType, setModalType] = useState<'edit' | 'changePassword' | null>(null);

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

  const handleClose = () => {
    setModalType(null);
    setError(null);
  };

  const handleEditSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const formData = new FormData(e.target as HTMLFormElement);

    if (user) {
      const result = await updateUser(user, formData);
      if (result?.isSuccess) {
        setSuccessMessage("Profile updated successfully!");
        handleClose();
        await fetchUserProfile();
      } else {
        setError(result?.message || "Update profile failed");
      }
    }
  };

  const handleChangePassword = async (e: React.FormEvent) => {
    e.preventDefault();
    const formData = new FormData(e.target as HTMLFormElement);
    const currentPassword = formData.get('currentPassword')?.toString();
    const newPassword = formData.get('newPassword')?.toString();
    const confirmPassword = formData.get('confirmPassword')?.toString();

    if (newPassword !== confirmPassword) {
      setError("New password and confirm password do not match");
      return;
    }

    if (user && currentPassword && newPassword) {
      const result = await updateUser(user, formData);
      if (result?.isSuccess) {
        setSuccessMessage("Password changed successfully!");
        handleClose();
      } else {
        setError(result?.message || "Change password failed");
      }
    }
  };

  const getAvatarUrl = () => {
    if (user?.detail?.UserPhoto) {
      return `data:image/jpeg;base64,${user.detail.UserPhoto}`;
    }
    return `https://api.dicebear.com/7.x/avataaars/svg?seed=${user?.detail?.FullName || user?.userID}`;
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return 'N/A';
    const options: Intl.DateTimeFormatOptions = { year: 'numeric', month: 'long', day: 'numeric' };
    return new Date(dateString).toLocaleDateString(undefined, options);
  };

  const getStatusStyle = (status: number) => {
    switch (status) {
      case 2: return 'bg-green-50 text-green-600';
      case 1: return 'bg-amber-50 text-amber-600';
      case 3: return 'bg-gray-50 text-gray-600';
      default: return 'bg-blue-50 text-blue-600';
    }
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
      {/* Success Message Toast */}
      {successMessage && (
        <div className="fixed top-4 right-4 z-50 bg-green-500 text-white px-6 py-3 rounded-xl shadow-lg">
          <div className="flex items-center gap-3">
            <span>{successMessage}</span>
            <button onClick={() => setSuccessMessage(null)} className="text-white hover:text-green-100">×</button>
          </div>
        </div>
      )}

      {/* Error Message Toast */}
      {error && !modalType && (
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
          <h1 className="text-3xl font-black text-gray-900 tracking-tight">My Profile</h1>
          <p className="text-gray-500 font-medium">View and manage your personal information.</p>
        </div>
        <div className="flex gap-3">
          <button
            onClick={() => setModalType('edit')}
            className="flex items-center gap-2 px-6 py-3 bg-blue-600 text-white rounded-2xl font-bold hover:bg-blue-700 transition-all shadow-lg shadow-blue-100 cursor-pointer"
          >
            <Edit2 size={20} />
            <span>Edit Profile</span>
          </button>
          <button
            onClick={() => setModalType('changePassword')}
            className="flex items-center gap-2 px-6 py-3 bg-gray-100 text-gray-700 rounded-2xl font-bold hover:bg-gray-200 transition-all cursor-pointer"
          >
            <Lock size={20} />
            <span>Change Password</span>
          </button>
        </div>
      </div>

      {/* Profile Card */}
      <div className="bg-white rounded-3xl shadow-sm border border-gray-50 overflow-hidden">
        {/* Profile Header */}
        <div className="bg-gradient-to-r from-blue-500 to-indigo-600 px-8 py-12">
          <div className="flex flex-col md:flex-row items-center gap-6">
            <img
              src={getAvatarUrl()}
              className="w-28 h-28 rounded-3xl shadow-xl border-4 border-white object-cover"
              alt={`${user.firstName} ${user.lastName}`}
            />
            <div className="text-center md:text-left">
              <h2 className="text-3xl font-black text-white">
                {user.firstName} {user.lastName}
              </h2>
              <p className="text-blue-100 font-medium mt-1">{user.email}</p>
              <span className={`inline-block mt-3 px-4 py-1.5 rounded-xl text-[10px] font-black uppercase tracking-widest ${getStatusStyle(user.status)}`}>
                {UserStatus.find(s => s.value === user.status?.toString())?.label || 'Unknown'}
              </span>
            </div>
          </div>
        </div>

        {/* Profile Details */}
        <div className="p-8">
          <h3 className="text-lg font-black text-gray-900 mb-6">Personal Information</h3>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="flex items-center gap-4 p-4 bg-gray-50 rounded-2xl">
              <div className="w-12 h-12 rounded-xl bg-blue-100 flex items-center justify-center">
                <User size={20} className="text-blue-600" />
              </div>
              <div>
                <p className="text-[11px] font-black text-gray-400 uppercase tracking-widest">Full Name</p>
                <p className="font-bold text-gray-900">{user.firstName} {user.lastName}</p>
              </div>
            </div>

            <div className="flex items-center gap-4 p-4 bg-gray-50 rounded-2xl">
              <div className="w-12 h-12 rounded-xl bg-green-100 flex items-center justify-center">
                <Mail size={20} className="text-green-600" />
              </div>
              <div>
                <p className="text-[11px] font-black text-gray-400 uppercase tracking-widest">Email</p>
                <p className="font-bold text-gray-900">{user.email || 'N/A'}</p>
              </div>
            </div>

            <div className="flex items-center gap-4 p-4 bg-gray-50 rounded-2xl">
              <div className="w-12 h-12 rounded-xl bg-purple-100 flex items-center justify-center">
                <Phone size={20} className="text-purple-600" />
              </div>
              <div>
                <p className="text-[11px] font-black text-gray-400 uppercase tracking-widest">Phone Number</p>
                <p className="font-bold text-gray-900">{user.phoneNumber || 'N/A'}</p>
              </div>
            </div>

            <div className="flex items-center gap-4 p-4 bg-gray-50 rounded-2xl">
              <div className="w-12 h-12 rounded-xl bg-amber-100 flex items-center justify-center">
                <Calendar size={20} className="text-amber-600" />
              </div>
              <div>
                <p className="text-[11px] font-black text-gray-400 uppercase tracking-widest">Birth Date</p>
                <p className="font-bold text-gray-900">{formatDate(user.birthDate)}</p>
              </div>
            </div>

            <div className="flex items-center gap-4 p-4 bg-gray-50 rounded-2xl md:col-span-2">
              <div className="w-12 h-12 rounded-xl bg-red-100 flex items-center justify-center">
                <MapPin size={20} className="text-red-600" />
              </div>
              <div>
                <p className="text-[11px] font-black text-gray-400 uppercase tracking-widest">Address</p>
                <p className="font-bold text-gray-900">{user.address || 'N/A'}</p>
              </div>
            </div>

            <div className="flex items-center gap-4 p-4 bg-gray-50 rounded-2xl">
              <div className="w-12 h-12 rounded-xl bg-cyan-100 flex items-center justify-center">
                <MapPin size={20} className="text-cyan-600" />
              </div>
              <div>
                <p className="text-[11px] font-black text-gray-400 uppercase tracking-widest">Location</p>
                <p className="font-bold text-gray-900">{user.location || 'N/A'}</p>
              </div>
            </div>

            <div className="flex items-center gap-4 p-4 bg-gray-50 rounded-2xl">
              <div className="w-12 h-12 rounded-xl bg-indigo-100 flex items-center justify-center">
                <Calendar size={20} className="text-indigo-600" />
              </div>
              <div>
                <p className="text-[11px] font-black text-gray-400 uppercase tracking-widest">Member Since</p>
                <p className="font-bold text-gray-900">{formatDate(user.createdAt)}</p>
              </div>
            </div>
          </div>
        </div>

        {/* Leave Balances Section */}
        {user?.leaveBalances && user.leaveBalances.length > 0 && (
          <div className="p-8 border-t border-gray-100">
            <div className="flex items-center gap-3 mb-6">
              <div className="w-10 h-10 rounded-xl bg-emerald-100 flex items-center justify-center">
                <Briefcase size={20} className="text-emerald-600" />
              </div>
              <h3 className="text-lg font-black text-gray-900">Leave Balances</h3>
            </div>
            <LeaveBalancesDisplay leaveBalances={user.leaveBalances} variant="card" />
          </div>
        )}
      </div>

      {/* Modals */}
      <UserFormModal
        isOpen={modalType === 'edit'}
        onClose={handleClose}
        onSubmit={handleEditSubmit}
        mode="edit"
        user={user}
        error={error}
      />

      <ChangePasswordModal
        isOpen={modalType === 'changePassword'}
        onClose={handleClose}
        onSubmit={handleChangePassword}
        error={error}
      />
    </div>
  );
}

export default UserProfile;
