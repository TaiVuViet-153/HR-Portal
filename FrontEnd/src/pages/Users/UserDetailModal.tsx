import React from 'react';
import { X, Mail, Phone, MapPin, Calendar, User, Briefcase } from 'lucide-react';
import type { UserWithDetail } from './UsersType';
import { DetailField, StatusBadge, DetailSection } from './UserDetailField';
import LeaveBalancesDisplay from './LeaveBalancesDisplay';

type UserDetailModalProps = {
  isOpen: boolean;
  onClose: () => void;
  user: UserWithDetail | null;
};

export default function UserDetailModal({ isOpen, onClose, user }: UserDetailModalProps) {
  if (!isOpen || !user) return null;

  const getAvatarUrl = () => {
    if (user.detail?.UserPhoto) {
      return `data:image/jpeg;base64,${user.detail.UserPhoto}`;
    }
    return `https://api.dicebear.com/7.x/avataaars/svg?seed=${user.detail?.FullName || user.userID}`;
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return 'N/A';
    const options: Intl.DateTimeFormatOptions = { year: 'numeric', month: 'long', day: 'numeric' };
    return new Date(dateString).toLocaleDateString(undefined, options);
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm">
      <div className="bg-white rounded-3xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-hidden">
        {/* Header */}
        <div className="bg-gradient-to-r from-blue-500 to-indigo-600 px-6 py-8 relative">
          <button
            onClick={onClose}
            className="absolute top-4 right-4 p-2 text-white/80 hover:text-white hover:bg-white/20 rounded-xl transition-all"
          >
            <X size={20} />
          </button>
          <div className="flex items-center gap-4">
            <img
              src={getAvatarUrl()}
              className="w-20 h-20 rounded-2xl border-4 border-white shadow-lg object-cover"
              alt={`${user.firstName} ${user.lastName}`}
            />
            <div>
              <h2 className="text-2xl font-bold text-white">
                {user.firstName} {user.lastName}
              </h2>
              <p className="text-blue-100">{user.email}</p>
              <div className="mt-2">
                <StatusBadge status={user.status} />
              </div>
            </div>
          </div>
        </div>

        {/* Content */}
        <div className="p-6 overflow-y-auto max-h-[calc(90vh-200px)]">
          <DetailSection title="Personal Information">
            <DetailField
              label="Full Name"
              value={`${user.firstName || ''} ${user.lastName || ''}`.trim() || 'N/A'}
              icon={<User size={18} />}
            />
            <DetailField
              label="Email"
              value={user.email}
              icon={<Mail size={18} />}
            />
            <DetailField
              label="Phone"
              value={user.phoneNumber}
              icon={<Phone size={18} />}
            />
            <DetailField
              label="Birth Date"
              value={formatDate(user.birthDate)}
              icon={<Calendar size={18} />}
            />
            <DetailField
              label="Address"
              value={user.address}
              icon={<MapPin size={18} />}
            />
            <DetailField
              label="Location"
              value={user.location}
              icon={<MapPin size={18} />}
            />
          </DetailSection>

          {/* Leave Balances Section */}
          {user.leaveBalances && user.leaveBalances.length > 0 && (
            <DetailSection title="Leave Balances">
              <LeaveBalancesDisplay leaveBalances={user.leaveBalances} variant="compact" />
            </DetailSection>
          )}

          <DetailSection title="Account Information">
            <DetailField
              label="User ID"
              value={user.userID}
            />
            <DetailField
              label="Username"
              value={user.userName}
            />
            <DetailField
              label="Member Since"
              value={formatDate(user.createdAt)}
              icon={<Calendar size={18} />}
            />
            {user.roles && user.roles.length > 0 && (
              <DetailField
                label="Roles"
                value={
                  <div className="flex flex-wrap gap-1">
                    {user.roles.map((role, idx) => (
                      <span
                        key={idx}
                        className="px-2 py-0.5 bg-indigo-50 text-indigo-600 rounded-md text-xs font-medium"
                      >
                        {role}
                      </span>
                    ))}
                  </div>
                }
                icon={<Briefcase size={18} />}
              />
            )}
          </DetailSection>
        </div>

        {/* Footer */}
        <div className="px-6 py-4 border-t border-gray-100 flex justify-end">
          <button
            onClick={onClose}
            className="px-6 py-2.5 bg-gray-100 text-gray-700 rounded-xl font-bold hover:bg-gray-200 transition-all"
          >
            Close
          </button>
        </div>
      </div>
    </div>
  );
}
