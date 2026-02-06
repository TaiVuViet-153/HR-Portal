import React from 'react';
import { X } from 'lucide-react';
import type { UserWithDetail } from './UsersType';
import SelectCommon from '@/components/common/SelectCommon';
import DatePickerCommon from '@/components/common/DatePickerCommon';
import { UserStatus } from './UsersData';
import { useAuth } from '@/core/auth/AuthContext';

interface UserFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (e: React.FormEvent) => void;
  mode: 'create' | 'edit';
  user: UserWithDetail | null;
  error: string | null;
}

export default function UserFormModal({ isOpen, onClose, onSubmit, mode, user, error }: UserFormModalProps) {
  if (!isOpen) return null;

  const { state } = useAuth();

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <div className="bg-white rounded-3xl shadow-2xl w-full max-w-2xl mx-4 max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-100">
          <h2 className="text-2xl font-black text-gray-900">
            {mode === 'create' ? 'Add New Employee' : 'Edit Employee'}
          </h2>
          <button onClick={onClose} className="p-2 hover:bg-gray-100 rounded-xl transition-all cursor-pointer">
            <X size={24} className="text-gray-500" />
          </button>
        </div>

        {/* Error Message */}
        {error && (
          <div className="mx-6 mt-4 p-4 bg-red-50 border border-red-200 rounded-xl text-red-600 text-sm">
            {error}
          </div>
        )}

        {/* Form */}
        <form onSubmit={onSubmit} className="p-6 space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {/* Username */}
            <div className="flex flex-col">
              <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">
                Username <span className="text-red-500">*</span>
              </label>
              <input
                name="userName"
                type="text"
                required
                disabled
                defaultValue={user?.userName || ''}
                placeholder="Enter username"
                className="px-4 py-3 bg-gray-50 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-transparent cursor-not-allowed"
              />
            </div>

            {/* Email */}
            <div className="flex flex-col">
              <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">
                Email <span className="text-red-500">*</span>
              </label>
              <input
                name="email"
                type="email"
                required
                defaultValue={user?.email || ''}
                placeholder="Enter email address"
                className="px-4 py-3 bg-gray-50 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            {/* First Name */}
            <div className="flex flex-col">
              <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">
                First Name
              </label>
              <input
                name="firstName"
                type="text"
                defaultValue={user?.firstName || ''}
                placeholder="Enter first name"
                className="px-4 py-3 bg-gray-50 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            {/* Last Name */}
            <div className="flex flex-col">
              <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">
                Last Name
              </label>
              <input
                name="lastName"
                type="text"
                defaultValue={user?.lastName || ''}
                placeholder="Enter last name"
                className="px-4 py-3 bg-gray-50 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            {/* Phone Number */}
            <div className="flex flex-col">
              <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">
                Phone Number
              </label>
              <input
                name="phoneNumber"
                type="tel"
                defaultValue={user?.phoneNumber || ''}
                placeholder="Enter phone number"
                className="px-4 py-3 bg-gray-50 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            {/* Birth Date */}
            <div className="flex flex-col">
              <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">
                Birth Date
              </label>
              <DatePickerCommon
                name="birthDate"
                defaultValue={user?.birthDate}
                disablePast={false}
              />
            </div>

            {/* Address */}
            <div className="flex flex-col md:col-span-2">
              <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">
                Address
              </label>
              <input
                name="address"
                type="text"
                defaultValue={user?.address || ''}
                placeholder="Enter address"
                className="px-4 py-3 bg-gray-50 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            {/* Location */}
            <div className="flex flex-col">
              <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">
                Location
              </label>
              <input
                name="location"
                type="text"
                defaultValue={user?.location || ''}
                placeholder="Enter location"
                className="px-4 py-3 bg-gray-50 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            {/* Time Zone */}
            <div className="flex flex-col">
              <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">
                Time Zone
              </label>
              <input
                name="timeZone"
                type="text"
                defaultValue={user?.timeZone || ''}
                placeholder="e.g., UTC+7"
                className="px-4 py-3 bg-gray-50 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            {/* Status - Only for edit mode */}
            {mode === 'edit' && state.user?.roles?.includes('ADMIN') && (
              <div className="flex flex-col">
                <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">
                  Status
                </label>
                <SelectCommon
                  name="status"
                  options={UserStatus}
                  defaultValue={user?.status?.toString()}
                  placeholder="Select status"
                />
              </div>
            )}
          </div>

          {/* Actions */}
          <div className="flex justify-end gap-3 pt-6 border-t border-gray-100">
            <button
              type="button"
              onClick={onClose}
              className="px-6 py-3 font-bold text-gray-600 bg-gray-100 rounded-xl hover:bg-gray-200 transition-all cursor-pointer"
            >
              Cancel
            </button>
            <button
              type="submit"
              className="px-6 py-3 font-bold text-white bg-blue-600 rounded-xl hover:bg-blue-700 transition-all shadow-lg shadow-blue-100 cursor-pointer"
            >
              {mode === 'create' ? 'Create Employee' : 'Save Changes'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
