import React from 'react';
import { X } from 'lucide-react';
import SelectCommon from '@/components/common/SelectCommon';
import { LeaveTypeOptions } from './BalancesData';
import type { GetBalancesResponse, LeaveBalance } from './BalancesType';

interface SelectedBalanceForEdit {
  user: GetBalancesResponse;
  balanceItem: LeaveBalance;
}

interface BalanceFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (e: React.FormEvent) => void;
  mode: 'create' | 'edit';
  selectedData: SelectedBalanceForEdit | null;
  error: string | null;
}

export default function BalanceFormModal({ isOpen, onClose, onSubmit, mode, selectedData, error }: BalanceFormModalProps) {
  if (!isOpen) return null;

  const user = selectedData?.user;
  const balanceItem = selectedData?.balanceItem;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <div className="bg-white rounded-3xl shadow-2xl w-full max-w-2xl mx-4 max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-100">
          <h2 className="text-2xl font-black text-gray-900">
            {mode === 'create' ? 'Add New Balance' : 'Edit Balance'}
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
                Username <span className="text-red-500">{mode === 'create' ? '*' : ''}</span>
              </label>
              <input
                name="userName"
                type="text"
                required
                disabled={mode === 'edit'}
                defaultValue={user?.userName || ''}
                placeholder="Enter username"
                className={`px-4 py-3 bg-gray-50 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                  mode === 'edit' ? 'cursor-not-allowed' : ''
                }`}
              />
            </div>

            {/* Leave Type */}
            <div className="flex flex-col">
              <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">
                Leave Type <span className="text-red-500">{mode === 'create' ? '*' : ''}</span>
              </label>
              <SelectCommon
                name="type"
                options={LeaveTypeOptions}
                defaultValue={balanceItem?.leaveType?.toString()}
                placeholder="Select leave type"
                isDisabled={mode === 'edit'}
              />
            </div>

            {/* Balance */}
            <div className="flex flex-col md:col-span-2">
              <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">
                Balance <span className="text-red-500">*</span>
              </label>
              <input
                name="balance"
                type="number"
                required
                step="0.5"
                defaultValue={balanceItem?.balance || ''}
                placeholder="Enter balance"
                className="px-4 py-3 bg-gray-50 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            {/* Year (Read-only) */}
            {mode === 'edit' && (
              <div className="flex flex-col">
                <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">
                  Year
                </label>
                <input
                  type="text"
                  disabled
                  defaultValue={balanceItem?.year || new Date().getFullYear()}
                  className="px-4 py-3 bg-gray-50 rounded-xl border border-gray-200 cursor-not-allowed"
                />
              </div>
            )}

            {/* Email (Read-only) */}
            {mode === 'edit' && (
              <div className="flex flex-col">
                <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">
                  Email
                </label>
                <input
                  type="email"
                  disabled
                  defaultValue={user?.email || ''}
                  className="px-4 py-3 bg-gray-50 rounded-xl border border-gray-200 cursor-not-allowed"
                />
              </div>
            )}

            {/* Created At (Read-only) */}
            {mode === 'edit' && (
              <div className="flex flex-col">
                <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">
                  Created At
                </label>
                <input
                  type="text"
                  disabled
                  defaultValue={new Date(user?.createdAt || new Date()).toLocaleDateString()}
                  className="px-4 py-3 bg-gray-50 rounded-xl border border-gray-200 cursor-not-allowed"
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
              {mode === 'create' ? 'Create Balance' : 'Save Changes'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
