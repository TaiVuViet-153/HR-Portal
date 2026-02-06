import { Calendar } from 'lucide-react';
import type { LeaveBalance } from './UsersType';
import { getTypeKey } from '../Requests/LeaveRequestsData';

type LeaveBalancesDisplayProps = {
  leaveBalances?: LeaveBalance[] | null;
  variant?: 'card' | 'compact';
};

const getLeaveTypeColor = (leaveType: string) => {
  const typeColors: Record<string, { bg: string; text: string; icon: string }> = {
    unpaid: { bg: 'bg-gray-50', text: 'text-gray-600', icon: 'bg-gray-100' },
    paid: { bg: 'bg-blue-50', text: 'text-blue-600', icon: 'bg-blue-100' },
    maternity: { bg: 'bg-pink-50', text: 'text-pink-600', icon: 'bg-pink-100' },
    wedding: { bg: 'bg-purple-50', text: 'text-purple-600', icon: 'bg-purple-100' },
    bereavement: { bg: 'bg-red-50', text: 'text-red-600', icon: 'bg-red-100' },
  };
  return typeColors[leaveType.toLowerCase()] || { bg: 'bg-green-50', text: 'text-green-600', icon: 'bg-green-100' };
};

const formatLeaveType = (leaveType: string) => {
  return leaveType
    .replace(/([A-Z])/g, ' $1')
    .replace(/^./, (str) => str.toUpperCase())
    .trim();
};

export default function LeaveBalancesDisplay({ leaveBalances, variant = 'card' }: LeaveBalancesDisplayProps) {
  if (!leaveBalances || leaveBalances.length === 0) {
    return (
      <div className="text-gray-400 text-sm italic">No leave balances available</div>
    );
  }

  // Group by year
  const groupedByYear = leaveBalances.reduce((acc, balance) => {
    const year = balance.year;
    if (!acc[year]) acc[year] = [];
    acc[year].push(balance);
    return acc;
  }, {} as Record<number, LeaveBalance[]>);

  const years = Object.keys(groupedByYear)
    .map(Number)
    .sort((a, b) => b - a);

  if (variant === 'compact') {
    return (
      <div className="space-y-3">
        {years.map((year) => (
          <div key={year}>
            <div className="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">
              {year}
            </div>
            <div className="flex flex-wrap gap-2">
              {groupedByYear[year].map((balance, idx) => {
                const leaveTypeName = getTypeKey(balance.leaveType);
                const colors = getLeaveTypeColor(leaveTypeName);
                return (
                  <span
                    key={`${year}-${balance.leaveType}-${idx}`}
                    className={`inline-flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-sm font-medium ${colors.bg} ${colors.text}`}
                  >
                    {formatLeaveType(leaveTypeName)}:
                    <span className="font-bold">{balance.balance}</span>
                  </span>
                );
              })}
            </div>
          </div>
        ))}
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {years.map((year) => (
        <div key={year}>
          <div className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-3">
            Leave Balances - {year}
          </div>
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
            {groupedByYear[year].map((balance, idx) => {
              const leaveTypeName = getTypeKey(balance.leaveType);
              const colors = getLeaveTypeColor(leaveTypeName);
              return (
                <div
                  key={`${year}-${balance.leaveType}-${idx}`}
                  className={`flex items-center gap-3 p-3 rounded-xl ${colors.bg}`}
                >
                  <div className={`w-10 h-10 rounded-lg ${colors.icon} flex items-center justify-center`}>
                    <Calendar size={18} className={colors.text} />
                  </div>
                  <div>
                    <p className="text-xs font-medium text-gray-500">
                      {formatLeaveType(leaveTypeName)}
                    </p>
                    <p className={`text-lg font-bold ${colors.text}`}>
                      {balance.balance} <span className="text-xs font-normal">days</span>
                    </p>
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      ))}
    </div>
  );
}
