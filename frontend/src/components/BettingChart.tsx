import React, { useState } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { Button } from '@/components/ui/button';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Card, CardContent } from '@/components/ui/card';

interface GameParams {
  serverSeed: string;
  clientSeed: string;
  initialBalance: string;
  nonce: string;
  strategy: string;
  game: string;
  initialBet?: string;
  initialBetPercentage?: string;
}

interface Bet {
  id: number;
  amount: number;
  payout: number;
  betResult: string;
  rawBetResult: number;
  balanceBeforeBet: number;
  balanceAfterBet: number;
}

interface GameResponse {
  bets: Bet[];
}

enum Strategies {
  Martingale = "martingale",
  ErrorProneMartingale = "errorProneMartingale",
  ModifiedMartingale = "modifiedMartingale",
  ErrorProneModifiedMartingale = "errorProneModifiedMartingale",
  FlatBetting = "flatBetting",
  ReverseMartingaleParoli = "reverseMartingaleParoli",
  FibonacciSystem = "fibonacciSystem",
}

enum GamblingGames {
  Pliko = "plinko",
}

interface BettingChartProps {
  showForm?: boolean;
  serverSeed?: string;
  clientSeed?: string;
  initialBalance?: string;
  nonce?: string;
  strategy?: string;
  game?: string;
  initialBet?: string;
  initialBetPercentage?: string;
}

const BettingChart = ({
  showForm = true,
  serverSeed: propServerSeed,
  clientSeed: propClientSeed,
  initialBalance: propInitialBalance,
  nonce: propNonce,
  strategy: propStrategy,
  game: propGame,
  initialBet: propInitialBet,
  initialBetPercentage: propInitialBetPercentage
}: BettingChartProps) => {
  const [data, setData] = useState<Bet[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  const [params, setParams] = useState<GameParams>({
    serverSeed: propServerSeed || "ZME1CHY7DD",
    clientSeed: propClientSeed || "f427ededbc8e385d285ffee7a61930721e6c044d46b6445c1b7e74575d06caac",
    initialBalance: propInitialBalance || "1000.00",
    nonce: propNonce || "229",
    strategy: propStrategy || Strategies.ModifiedMartingale,
    game: propGame || GamblingGames.Pliko,
    initialBet: propInitialBet || "10",
    initialBetPercentage: propInitialBetPercentage || ""
  });

  React.useEffect(() => {
    if (!showForm) {
      handlePlay();
    }
  }, []); // Only run once on mount when showForm is false

  const handleParamChange = (field: keyof GameParams, value: string) => {
    setParams(prev => ({
      ...prev,
      [field]: value
    }));
  };

  const handlePlay = async () => {
    setLoading(true);
    setError(null);
    
    try {
      const requestBody = {
        ...params,
        nonce: Number(params.nonce),
        initialBet: params.initialBet ? Number(params.initialBet) : undefined,
        initialBetPercentage: params.initialBetPercentage ? Number(params.initialBetPercentage) : undefined
      };

      const response = await fetch('http://localhost:9091/play', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json'
        },
        body: JSON.stringify(requestBody)
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`HTTP error! status: ${response.status} - ${errorText}`);
      }

      const result: GameResponse = await response.json();
      setData(result.bets);
    } catch (err) {
      console.error('Fetch error:', err);
      const errorMessage = err instanceof Error ? err.message : 'An unknown error occurred';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(value);
  };

  return (
    <div className="w-full space-y-4 p-4">
      {error && (
        <Alert variant="destructive">
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}

      {showForm && (
        <Card>
          <CardContent className="pt-6">
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              <div className="space-y-2">
                <Label htmlFor="initialBalance">Initial Balance</Label>
                <Input
                  id="initialBalance"
                  type="number"
                  step="0.01"
                  value={params.initialBalance}
                  onChange={(e) => handleParamChange('initialBalance', e.target.value)}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="initialBet">Initial Bet</Label>
                <Input
                  id="initialBet"
                  type="number"
                  step="0.01"
                  value={params.initialBet}
                  onChange={(e) => handleParamChange('initialBet', e.target.value)}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="initialBetPercentage">Initial Bet Percentage</Label>
                <Input
                  id="initialBetPercentage"
                  type="number"
                  step="0.01"
                  value={params.initialBetPercentage}
                  onChange={(e) => handleParamChange('initialBetPercentage', e.target.value)}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="game">Game</Label>
                <Select
                  value={params.game}
                  onValueChange={(value) => handleParamChange('game', value)}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Select Game" />
                  </SelectTrigger>
                  <SelectContent>
                    {Object.values(GamblingGames).map((game) => (
                      <SelectItem key={game} value={game}>
                        {game.charAt(0).toUpperCase() + game.slice(1).replace(/([A-Z])/g, ' $1')}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              
              <div className="space-y-2">
                <Label htmlFor="strategy">Strategy</Label>
                <Select
                  value={params.strategy}
                  onValueChange={(value) => handleParamChange('strategy', value)}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Select strategy" />
                  </SelectTrigger>
                  <SelectContent>
                    {Object.values(Strategies).map((strategy) => (
                      <SelectItem key={strategy} value={strategy}>
                        {strategy.charAt(0).toUpperCase() + strategy.slice(1).replace(/([A-Z])/g, ' $1')}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-2"></div>

              <div className="space-y-2">
                <Label htmlFor="serverSeed">Server Seed</Label>
                <Input
                  id="serverSeed"
                  value={params.serverSeed}
                  onChange={(e) => handleParamChange('serverSeed', e.target.value)}
                />
              </div>
              
              <div className="space-y-2">
                <Label htmlFor="clientSeed">Client Seed</Label>
                <Input
                  id="clientSeed"
                  value={params.clientSeed}
                  onChange={(e) => handleParamChange('clientSeed', e.target.value)}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="nonce">Nonce</Label>
                <Input
                  id="nonce"
                  value={params.nonce}
                  onChange={(e) => handleParamChange('nonce', e.target.value)}
                />
              </div>
            </div>

            <div className="mt-4">
              <Button 
                onClick={handlePlay}
                disabled={loading}
                className="w-full md:w-auto"
              >
                {loading ? 'Playing...' : 'Play Game'}
              </Button>
            </div>
          </CardContent>
        </Card>
      )}

      {data.length > 0 && (
        <>
          <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
            <div className="p-4 border rounded-lg bg-white shadow">
              <h3 className="text-lg font-semibold mb-2">Final Balance</h3>
              <p className="text-2xl">{formatCurrency(data[data.length - 1].balanceAfterBet)}</p>
            </div>
            <div className="p-4 border rounded-lg bg-white shadow">
              <h3 className="text-lg font-semibold mb-2">Total Bets</h3>
              <p className="text-2xl">{data.length}</p>
            </div>
            <div className="p-4 border rounded-lg bg-white shadow">
              <h3 className="text-lg font-semibold mb-2">Profit/Loss</h3>
              <p className={`text-2xl ${data[data.length - 1].balanceAfterBet - data[0].balanceBeforeBet > 0 ? 'text-green-600' : 'text-red-600'}`}>
                {formatCurrency(data[data.length - 1].balanceAfterBet - data[0].balanceBeforeBet)}
              </p>
            </div>
            <div className="p-4 border rounded-lg bg-white shadow">
              <h3 className="text-lg font-semibold mb-2">Max Bet</h3>
              <p className="text-2xl">{formatCurrency(Math.max(...data.map(bet => bet.amount)))}</p>
            </div>
            <div className="p-4 border rounded-lg bg-white shadow">
              <h3 className="text-lg font-semibold mb-2">Min Bet</h3>
              <p className="text-2xl">{formatCurrency(Math.min(...data.map(bet => bet.amount)))}</p>
            </div>
          </div>

          <div className="h-96 w-full border rounded-lg bg-white p-4">
            <ResponsiveContainer width="100%" height="100%">
              <LineChart
                data={data}
                margin={{
                  top: 5,
                  right: 30,
                  left: 20,
                  bottom: 5,
                }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis 
                  dataKey="id"
                />
                <YAxis 
                  yAxisId="left"
                  domain={['dataMin - 100', 'dataMax + 100']}
                  tickFormatter={formatCurrency}
                />
                <YAxis 
                  yAxisId="right"
                  orientation="right"
                  domain={[0, 'dataMax + 1']}
                  allowDecimals={false}
                />
                <Tooltip 
                  formatter={(value: any, name: string) => {
                    if (name === 'Balance' || name === 'Bet Amount') {
                      return formatCurrency(value);
                    }
                    if (name === 'Raw Result') {
                      return value.toFixed(2);
                    }
                    return value;
                  }}
                  labelFormatter={(label) => `Bet #${label}`}
                />
                <Legend />
                <Line
                  type="monotone"
                  dataKey="balanceAfterBet"
                  stroke="#8884d8"
                  strokeOpacity={0.9}
                  strokeWidth={3}
                  name="Balance"
                  dot={false}
                  yAxisId="left"
                />
                <Line
                  type="monotone"
                  dataKey="amount"
                  stroke="#38761d"
                  strokeOpacity={0.9}
                  strokeWidth={3}
                  name="Bet Amount"
                  dot={false}
                  yAxisId="left"
                />
                <Line
                  type="monotone"
                  dataKey="rawBetResult"
                  stroke="#ff7300"
                  strokeOpacity={0.4}
                  name="Multiplier"
                  yAxisId="right"
                />
                <Line
                  type="step"
                  strokeOpacity={0.8}
                  strokeWidth={0.5}
                  dataKey={(data) => data.betResult === "won" ? 1 : data.betResult === "lost" ? -1 : 0}
                  stroke="#0047fa"
                  name="Result"
                  yAxisId="right"
                  dot
                />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </>
      )}
    </div>
  );
};

export default BettingChart;